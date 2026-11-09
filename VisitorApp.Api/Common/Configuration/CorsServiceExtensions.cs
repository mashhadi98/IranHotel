namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring Cross-Origin Resource Sharing (CORS)
/// </summary>
public static class CorsServiceExtensions
{
    /// <summary>
    /// Adds CORS services with predefined policy to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy", policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
        }));

        return services;
    }
}