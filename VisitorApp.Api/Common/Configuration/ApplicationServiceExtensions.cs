using FluentValidation;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring application layer services
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Adds MediatR and FluentValidation services from the Application assembly
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add MediatR for handling commands, queries, and notifications
        services.AddMediatR(config => config
            .RegisterServicesFromAssemblyContaining(typeof(AssemblyReference)));

        // Add FluentValidation validators from the Application assembly
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        return services;
    }
}