using VisitorApp.Domain.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using VisitorApp.API.Common.Models;

namespace VisitorApp.API.Common.Middlewares;

/// <summary>
/// Global error handling middleware that provides standardized error responses and logging
/// </summary>
public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment environment;
    private readonly ILogger<GlobalErrorHandlingMiddleware> logger;

    public GlobalErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment environment, ILogger<GlobalErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.environment = environment;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        // Log the error with Serilog
        Log.Error(exception,
            "An error occurred while processing request {RequestPath} with TraceId {TraceId}. User: {UserName}",
            context.Request.Path,
            traceId,
            context.User.Identity?.Name ?? "Anonymous");

        // Determine response based on exception type
        var errorResponse = exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, traceId),
            DbUpdateConcurrencyException concurrencyEx => CreateConcurrencyErrorResponse(concurrencyEx, traceId),
            EntityNotFoundException entityNotFoundEx => CreateEntityNotFoundErrorResponse(entityNotFoundEx, traceId),
            BusinessLogicException businessLogicEx => CreateBusinessLogicErrorResponse(businessLogicEx, traceId),
            DomainException domainEx => CreateDomainErrorResponse(domainEx, traceId),
            ArgumentException argumentEx => CreateBadRequestErrorResponse(argumentEx, traceId),
            UnauthorizedAccessException => CreateUnauthorizedErrorResponse(traceId),
            KeyNotFoundException => CreateNotFoundErrorResponse(traceId),
            InvalidOperationException invalidOpEx => CreateBadRequestErrorResponse(invalidOpEx, traceId),
            _ => CreateInternalServerErrorResponse(exception, traceId)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.StatusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private StandardResponse CreateValidationErrorResponse(ValidationException exception, string traceId)
    {
        var errors = new Dictionary<string, object>();

        // Extract validation errors if available
        if (exception.Data.Count > 0)
        {
            foreach (var key in exception.Data.Keys)
            {
                errors[key.ToString()!] = exception.Data[key]!;
            }
        }
        string messageValidationTitle = Environment.GetEnvironmentVariable("ErrorResponse__Validation__Title") ?? "Validation failed";
        string messageValidationDetails = Environment.GetEnvironmentVariable("ErrorResponse__Validation__Details") ?? "One or more validation errors occurred.";
        var response = new StandardResponse(); return response.StandardErrorResponse(
            message: messageValidationTitle,
            statusCode: (int)HttpStatusCode.BadRequest,
            details: environment.IsDevelopment() ? exception.Message : messageValidationDetails,
            traceId: traceId,
            validationErrors: errors.Count > 0 ? errors : null
        );
    }

    private StandardResponse CreateBadRequestErrorResponse(Exception exception, string traceId)
    {
        string messageBadRequestTitle = Environment.GetEnvironmentVariable("ErrorResponse__BadRequest__Title") ?? "Bad Request";
        string messageBadRequestDetails = Environment.GetEnvironmentVariable("ErrorResponse__BadRequest__Details") ?? "The request was invalid.";

        var response = new StandardResponse(); 
        return response.StandardErrorResponse(
            message: messageBadRequestTitle,
            statusCode: (int)HttpStatusCode.BadRequest,
            details: environment.IsDevelopment() ? exception.Message : messageBadRequestDetails,
            traceId: traceId
        );
    }

    private StandardResponse CreateUnauthorizedErrorResponse(string traceId)
    {
        string messageUnauthorizedTitle = Environment.GetEnvironmentVariable("ErrorResponse__Unauthorized__Title") ?? "Unauthorized";
        string messageUnauthorizedDetails = Environment.GetEnvironmentVariable("ErrorResponse__Unauthorized__Details") ?? "Access denied. Please provide valid authentication credentials.";

        var response = new StandardResponse();
        return response.StandardErrorResponse(
            message: messageUnauthorizedTitle,
            statusCode: (int)HttpStatusCode.Unauthorized,
            details: messageUnauthorizedDetails,
            traceId: traceId
        );
    }

    private StandardResponse CreateNotFoundErrorResponse(string traceId)
    {
        string messageNotFoundTitle = Environment.GetEnvironmentVariable("ErrorResponse__NotFound__Title") ?? "Not Found";
        string messageNotFoundDetails = Environment.GetEnvironmentVariable("ErrorResponse__NotFound__Details") ?? "The requested resource was not found.";

        var response = new StandardResponse();
        return response.StandardErrorResponse(
            message: messageNotFoundTitle,
            statusCode: (int)HttpStatusCode.NotFound,
            details: messageNotFoundDetails,
            traceId: traceId
        );
    }

    private StandardResponse CreateEntityNotFoundErrorResponse(EntityNotFoundException exception, string traceId)
    {
        string messageEntityNotFoundTitle = Environment.GetEnvironmentVariable("ErrorResponse__EntityNotFound__Title") ?? "Resource Not Found";
        string messageEntityNotFoundDetails = Environment.GetEnvironmentVariable("ErrorResponse__EntityNotFound__Details") ?? "The requested resource was not found.";

        var response = new StandardResponse(); 
        return response.StandardErrorResponse(
            message: messageEntityNotFoundTitle,
            statusCode: (int)HttpStatusCode.NotFound,
            details: environment.IsDevelopment() ? exception.Message : messageEntityNotFoundDetails,
            traceId: traceId
        );
    }

    private StandardResponse CreateBusinessLogicErrorResponse(BusinessLogicException exception, string traceId)
    {
        string messageBusinessLogicTitle = Environment.GetEnvironmentVariable("ErrorResponse__BusinessLogic__Title") ?? "Business Logic Error";

        var response = new StandardResponse(); 
        return response.StandardErrorResponse(
            message: messageBusinessLogicTitle,
            statusCode: (int)HttpStatusCode.BadRequest,
            details: exception.Message, // Business logic errors can be shown to users
            traceId: traceId
        );
    }

    private StandardResponse CreateDomainErrorResponse(DomainException exception, string traceId)
    {
        string messageDomainErrorTitle = Environment.GetEnvironmentVariable("ErrorResponse__DomainError__Title") ?? "Domain Error";

        var response = new StandardResponse(); 
        return response.StandardErrorResponse(
            message: messageDomainErrorTitle,
            statusCode: (int)HttpStatusCode.BadRequest,
            details: exception.Message, // Domain errors can be shown to users
            traceId: traceId
        );
    }

    private StandardResponse CreateConcurrencyErrorResponse(DbUpdateConcurrencyException exception, string traceId)
    {
        string messageConcurrencyTitle = Environment.GetEnvironmentVariable("ErrorResponse__Concurrency__Title") ?? "Concurrency Conflict";
        string messageConcurrencyDetails = Environment.GetEnvironmentVariable("ErrorResponse__Concurrency__Details") ?? "The record has been modified by another user. Please refresh and try again.";

        // Log concurrency details for debugging
        logger.LogWarning("Concurrency conflict detected for entities: {EntityNames}. TraceId: {TraceId}",
            string.Join(", ", exception.Entries.Select(e => e.Entity.GetType().Name)),
            traceId);

        var response = new StandardResponse(); 
        return response.StandardErrorResponse(
            message: messageConcurrencyTitle,
            statusCode: (int)HttpStatusCode.Conflict, // 409 Conflict
            details: environment.IsDevelopment() 
                ? $"{exception.Message}. Affected entities: {string.Join(", ", exception.Entries.Select(e => e.Entity.GetType().Name))}"
                : messageConcurrencyDetails,
            traceId: traceId
        );
    }

    private StandardResponse CreateInternalServerErrorResponse(Exception exception, string traceId)
    {
        string messageInternalServerErrorTitle = Environment.GetEnvironmentVariable("ErrorResponse__InternalServerError__Title") ?? "Internal Server Error";
        string messageInternalServerErrorDetails = Environment.GetEnvironmentVariable("ErrorResponse__InternalServerError__Details") ?? "An error occurred while processing your request. Please try again later.";

        var response = new StandardResponse();
        return response.StandardErrorResponse(
            message: messageInternalServerErrorTitle,
            statusCode: (int)HttpStatusCode.InternalServerError,
            details: environment.IsDevelopment()
                ? $"{exception.Message}\n{exception.StackTrace}"
                : messageInternalServerErrorDetails,
            traceId: traceId
        );
    }
}