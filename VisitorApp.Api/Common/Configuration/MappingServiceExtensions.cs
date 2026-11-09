namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring object mapping services
/// </summary>
public static class MappingServiceExtensions
{
    /// <summary>
    /// Adds AutoMapper with configured profiles to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMappingServices(this IServiceCollection services)
    {
      
        services.AddAutoMapper(cfg =>
        {
            // Add your license key here if you have one
            // cfg.LicenseKey = "your-license-key";
        }, typeof(Program).Assembly, AssemblyReference.Assembly);

        return services;
    }
}