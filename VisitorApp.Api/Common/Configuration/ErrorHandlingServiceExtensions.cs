using VisitorApp.API.Common.Middlewares;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring error handling services and middleware
/// </summary>
public static class ErrorHandlingServiceExtensions
{
    /// <summary>
    /// Adds global error handling middleware to the application pipeline
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        return app;
    }

    /// <summary>
    /// Adds response wrapper middleware to standardize all API responses
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseStandardResponseWrapper(this WebApplication app)
    {
        app.UseMiddleware<ResponseWrapperMiddleware>();
        return app;
    }
} 