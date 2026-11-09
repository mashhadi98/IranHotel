using System.Text.Json;
using VisitorApp.API.Common.Models;

namespace VisitorApp.API.Common.Middlewares;

/// <summary>
/// Middleware to wrap successful responses in a standard format
/// </summary>
public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ResponseWrapperMiddleware> logger;

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    private static readonly HashSet<string> StaticExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        ".png", ".jpg", ".jpeg", ".gif", ".webp", ".svg", ".ico", ".bmp", ".tif", ".tiff", ".avif",
        // Fonts
        ".woff", ".woff2", ".ttf", ".otf", ".eot",
        // Scripts & Styles
        ".js", ".mjs", ".map", ".css",
        // Docs & other binaries
        ".pdf", ".txt", ".zip", ".rar",
        // Videos/Audios (اگر داشتی)
        ".mp4", ".webm", ".ogg", ".mp3", ".wav"
    };

    private static readonly string[] KnownStaticSegments =
    {
        "/swagger", "/health", "/_framework",
        "/css", "/js", "/images", "/img", "/uploads",
        "/favicon.ico"
    };

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip wrapping for certain endpoints
        if (ShouldSkipWrapping(context))
        {
            await next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        // Only wrap successful responses (2xx status codes)
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            await WrapSuccessResponse(context, responseBody, originalBodyStream);
        }
        else
        {
            // For error responses, just copy the response as-is (handled by GlobalErrorHandlingMiddleware)
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task WrapSuccessResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

        object data;
        
        // Try to parse existing JSON response, fallback to raw string if not JSON
        try
        {
            if (!string.IsNullOrEmpty(responseContent))
            {
                data = JsonSerializer.Deserialize<object>(responseContent) ?? new { };
            }
            else
            {
                data = new { };
            }
        }
        catch
        {
            // If not valid JSON, wrap the raw content
            data = string.IsNullOrEmpty(responseContent) ? new { } : responseContent;
        }

        var standardResponse = new StandardResponse();
        standardResponse = standardResponse.StandardSuccessResponse
        (
            data: data,
            statusCode: context.Response.StatusCode,
            traceId: context.TraceIdentifier
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var wrappedResponse = JsonSerializer.Serialize(standardResponse, jsonOptions);
        
        // Update content type and length
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = null; // Let the framework calculate this
        
        // Write the wrapped response
        context.Response.Body = originalBodyStream;
        await context.Response.WriteAsync(wrappedResponse);
    }

    public static bool ShouldSkipWrapping(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var lower = path.ToLowerInvariant();

        // 1) مسیرهای شناخته‌شده‌ی استاتیک
        if (KnownStaticSegments.Any(s => lower.Contains(s)))
            return true;

        // 2) بر اساس پسوند فایل (تصویر، فونت، css/js و …)
        if (Path.HasExtension(path))
        {
            var ext = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(ext) && StaticExtensions.Contains(ext))
                return true;
        }

        // 3) اگر کلاینت explicitly فقط تصویر خواسته (Accept header)
        //    (در APIهای فایل‌محور مفید است)
        if (context.Request.Headers.TryGetValue("Accept", out var accept) &&
            accept.Any(a => a.Contains("image/", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        // 4) اگر از قبل ContentType ست شده و باینری/تصویری است
        if (!string.IsNullOrEmpty(context.Response.ContentType) &&
            (context.Response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
             context.Response.ContentType.StartsWith("font/", StringComparison.OrdinalIgnoreCase) ||
             context.Response.ContentType.StartsWith("text/css", StringComparison.OrdinalIgnoreCase) ||
             context.Response.ContentType.StartsWith("text/javascript", StringComparison.OrdinalIgnoreCase) ||
             context.Response.ContentType.StartsWith("application/javascript", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        // 5) صفحات Razor/HTML را هم wrap نکن (در صورت نیاز)
        if (context.Response.ContentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    }
} 