namespace VisitorApp.API.Common.Models;

/// <summary>
/// Standard response wrapper for all API responses
/// </summary>
public record StandardResponse
{
    public string TraceId { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public object Data { get; private set; }
    public ErrorDetails Error { get; private set; }

    public StandardResponse()
    {
        
    }
    protected StandardResponse(int statusCode, string traceId = "")
    {
        StatusCode = statusCode;
        TraceId = traceId;
        Timestamp = DateTime.UtcNow;
    }

    public StandardResponse StandardErrorResponse(string message, int statusCode = 500, string? details = null, string traceId = "", Dictionary<string, object>? validationErrors = null)
    {
        var result = new StandardResponse(statusCode, traceId);
        result.Error = new ErrorDetails(message, details, validationErrors);
        return result;
    }
    public StandardResponse StandardSuccessResponse(object data, int statusCode = 200, string traceId = "")
    {
        var result = new StandardResponse(statusCode, traceId);
        result.Data = data;
        return result;
    }
}

/// <summary>
/// Error details nested object
/// </summary>
public sealed record ErrorDetails
{
    public string Message { get; init; } = string.Empty;
    public string? Details { get; init; }
    public Dictionary<string, object>? ValidationErrors { get; init; }

    public ErrorDetails(string message, string? details = null, Dictionary<string, object>? validationErrors = null)
    {
        Message = message;
        Details = details;
        ValidationErrors = validationErrors;
    }
}