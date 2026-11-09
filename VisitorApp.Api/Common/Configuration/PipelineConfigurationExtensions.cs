using FastEndpoints.Swagger;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring the HTTP request pipeline
/// </summary>
public static class PipelineConfigurationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with appropriate middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configure response wrapper middleware (should come before error handling)
        app.UseStandardResponseWrapper();
        
        // Configure global error handling middleware
        app.UseGlobalErrorHandling();

        // Configure HTTPS and security
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        // Configure CORS
        app.UseCors("CorsPolicy");

        // Configure security and routing
        app.UseHttpsRedirection();

        // Configure authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Configure FastEndpoints under the /api prefix
        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
            config.Versioning.PrependToRoute = false;
        });

        // Enable Swagger UI in development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerGen();
        }

        return app;
    }
}