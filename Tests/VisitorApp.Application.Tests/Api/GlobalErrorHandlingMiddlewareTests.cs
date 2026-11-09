using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using VisitorApp.API.Common.Middlewares;
using VisitorApp.Domain.Common.Exceptions;

namespace VisitorApp.Application.Tests.Api;

public class GlobalErrorHandlingMiddlewareTests
{
    [Theory]
    [InlineData(typeof(ArgumentException), 400)]
    [InlineData(typeof(ValidationException), 400)]
    [InlineData(typeof(FluentValidation.ValidationException), 400)]
    [InlineData(typeof(BusinessLogicException), 400)]
    [InlineData(typeof(KeyNotFoundException), 404)]
    [InlineData(typeof(EntityNotFoundException), 404)]
    [InlineData(typeof(DbUpdateConcurrencyException), 409)]
    [InlineData(typeof(Exception), 500)]
    public async Task InvokeAsync_ReturnsStandardErrorResponse(Type exceptionType, int expectedStatusCode)
    {
        var exception = CreateException(exceptionType);
        var context = CreateContext();
        var middleware = CreateMiddleware(exception, "Production");

        await middleware.InvokeAsync(context);

        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        using var response = await JsonDocument.ParseAsync(
            context.Response.Body,
            cancellationToken: TestContext.Current.CancellationToken);
        var root = response.RootElement;
        Assert.Equal(expectedStatusCode, root.GetProperty("statusCode").GetInt32());
        Assert.Equal("test-trace-id", root.GetProperty("traceId").GetString());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("data").ValueKind);
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("error").GetProperty("message").GetString()));
    }

    [Fact]
    public async Task InvokeAsync_DoesNotExposeUnexpectedExceptionDetailsInProduction()
    {
        var context = CreateContext();
        var middleware = CreateMiddleware(new Exception("sensitive server detail"), "Production");

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var response = await JsonDocument.ParseAsync(
            context.Response.Body,
            cancellationToken: TestContext.Current.CancellationToken);
        var details = response.RootElement.GetProperty("error").GetProperty("details").GetString();
        Assert.DoesNotContain("sensitive server detail", details);
    }

    [Fact]
    public async Task InvokeAsync_IncludesFluentValidationErrorsByProperty()
    {
        var exception = new FluentValidation.ValidationException(new[]
        {
            new FluentValidation.Results.ValidationFailure("Quantity", "Quantity must be positive")
        });
        var context = CreateContext();
        var middleware = CreateMiddleware(exception, "Production");

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var response = await JsonDocument.ParseAsync(
            context.Response.Body,
            cancellationToken: TestContext.Current.CancellationToken);
        var errors = response.RootElement.GetProperty("error").GetProperty("validationErrors");
        Assert.Equal("Quantity must be positive", errors.GetProperty("Quantity")[0].GetString());
    }

    [Fact]
    public async Task InvokeAsync_ReplacesPartialBodyWithStandardErrorResponse()
    {
        var context = CreateContext();
        var middleware = new GlobalErrorHandlingMiddleware(
            async httpContext =>
            {
                await httpContext.Response.WriteAsync("partial response");
                throw new ArgumentException("invalid request");
            },
            CreateEnvironment("Production"),
            NullLogger<GlobalErrorHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var response = await JsonDocument.ParseAsync(
            context.Response.Body,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("test-trace-id", response.RootElement.GetProperty("traceId").GetString());
        Assert.DoesNotContain("partial response", response.RootElement.GetRawText());
    }

    [Fact]
    public async Task InvokeAsync_RethrowsWhenResponseHasAlreadyStarted()
    {
        var exception = new InvalidOperationException("response already started");
        var context = CreateContext();
        var responseFeature = new StartedResponseFeature();
        context.Features.Set<IHttpResponseFeature>(responseFeature);
        var middleware = CreateMiddleware(exception, "Production");

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.Same(exception, thrown);
    }

    private static GlobalErrorHandlingMiddleware CreateMiddleware(Exception exception, string environmentName) =>
        new(_ => Task.FromException(exception), CreateEnvironment(environmentName),
            NullLogger<GlobalErrorHandlingMiddleware>.Instance);

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "test-trace-id";
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static IWebHostEnvironment CreateEnvironment(string environmentName) =>
        new TestWebHostEnvironment { EnvironmentName = environmentName };

    private static Exception CreateException(Type exceptionType)
    {
        if (exceptionType == typeof(ArgumentException))
            return new ArgumentException("invalid request");
        if (exceptionType == typeof(ValidationException))
            return new ValidationException("validation failed");
        if (exceptionType == typeof(FluentValidation.ValidationException))
            return new FluentValidation.ValidationException(
                new[] { new FluentValidation.Results.ValidationFailure("Quantity", "Quantity must be positive") });
        if (exceptionType == typeof(BusinessLogicException))
            return new BusinessLogicException("business rule failed");
        if (exceptionType == typeof(KeyNotFoundException))
            return new KeyNotFoundException("not found");
        if (exceptionType == typeof(EntityNotFoundException))
            return new EntityNotFoundException("Order", Guid.NewGuid());
        if (exceptionType == typeof(DbUpdateConcurrencyException))
            return new DbUpdateConcurrencyException("concurrency conflict");

        return new Exception("unexpected error");
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = nameof(GlobalErrorHandlingMiddlewareTests);
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = AppContext.BaseDirectory;
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class StartedResponseFeature : IHttpResponseFeature
    {
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public string? ReasonPhrase { get; set; }
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public Stream Body { get; set; } = Stream.Null;
        public bool HasStarted => true;
        public void OnStarting(Func<object, Task> callback, object state) { }
        public void OnCompleted(Func<object, Task> callback, object state) { }
    }
}
