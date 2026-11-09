using FastEndpoints.Swagger;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring API documentation and endpoints
/// </summary>
public static class ApiDocumentationServiceExtensions
{
    /// <summary>
    /// Adds FastEndpoints and Swagger documentation services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApiDocumentationServices(this IServiceCollection services)
    {
        // Configure FastEndpoints with Swagger documentation
        services.AddFastEndpoints().SwaggerDocument(options =>
        {
            options.ShortSchemaNames = true;
            options.MaxEndpointVersion = 1;
            options.DocumentSettings = settings =>
            {
                settings.DocumentName = "Release 1.0";
                settings.Title = "VisitorApp API";
                settings.Version = "v1.0";
            };
        })
        .SwaggerDocument(options =>
        {
            options.ShortSchemaNames = true;
            options.MaxEndpointVersion = 2;
            options.DocumentSettings = settings =>
            {
                settings.DocumentName = "Release 2.0";
                settings.Title = "VisitorApp API";
                settings.Version = "v2.0";
            };
        });

        // Add endpoints API explorer for swagger
        services.AddEndpointsApiExplorer();

        return services;
    }
}