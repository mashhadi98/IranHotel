using Scrutor;
using VisitorApp.Application.Common.Messaging;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring dependency injection and service scanning
/// </summary>
public static class DependencyInjectionServiceExtensions
{
    /// <summary>
    /// Adds automatic dependency injection scanning for Infrastructure and Persistence layers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services)
    {
        // Register all validator services as Transient
        services.Scan(selector => selector
            .FromAssemblies(
                VisitorApp.API.AssemblyReference.Assembly,
                VisitorApp.Application.AssemblyReference.Assembly,
                VisitorApp.Domain.AssemblyReference.Assembly,
                VisitorApp.Infrastructure.AssemblyReference.Assembly,
                VisitorApp.Persistence.AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IValidatorService<>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(selector => selector
            .FromAssemblies(
                VisitorApp.API.AssemblyReference.Assembly,
                VisitorApp.Application.AssemblyReference.Assembly,
                VisitorApp.Domain.AssemblyReference.Assembly,
                VisitorApp.Infrastructure.AssemblyReference.Assembly,
                VisitorApp.Persistence.AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IValidatorService<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(selector => selector
            .FromAssemblies(
                VisitorApp.Application.AssemblyReference.Assembly,
                VisitorApp.Infrastructure.AssemblyReference.Assembly,
                VisitorApp.Persistence.AssemblyReference.Assembly)
            .AddClasses(classes => classes
                .Where(type =>
                    type.Name is not "EfRepository" and not "ReadRepository" and not "WriteRepository" &&
                    (
                        (type.Name.EndsWith("Service") && !type.Name.EndsWith("ValidatorService")) ||
                        type.Name.EndsWith("Repository") ||
                        type.Name.EndsWith("Handler") ||
                        (type.IsClass && type.GetInterfaces().Any(i =>
                            i.Name.StartsWith("I") &&
                            (i.Name.EndsWith("Service") || i.Name.EndsWith("Repository"))
                        ))
                    )
                )
                .NotInNamespaces(
                    "VisitorApp.Persistence.Specifications",
                    "VisitorApp.API.Common.Models",
                    "VisitorApp.Domain.Common.Exceptions",
                    "VisitorApp.Application.Common.Contracts"
                )
            )
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register specific services manually
        services.AddScoped<VisitorApp.Application.Common.Services.ICurrentUserService, VisitorApp.API.Common.Services.CurrentUserService>();
        services.AddScoped(typeof(VisitorApp.Application.Common.Interfaces.IRepository<>), typeof(VisitorApp.Infrastructure.Common.Repository.EfRepository<>));
        services.AddScoped(typeof(VisitorApp.Application.Common.Interfaces.IRepository<,>), typeof(VisitorApp.Infrastructure.Common.Repository.EfRepository<,>));
        // Register HttpContextAccessor for CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
}