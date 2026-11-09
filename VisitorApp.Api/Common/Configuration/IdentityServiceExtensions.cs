using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Persistence.Common.Context;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring Identity and OpenIddict services
/// </summary>
public static class IdentityServiceExtensions
{
    /// <summary>
    /// Configures Identity services with custom ApplicationUser and ApplicationRole
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // Configure Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Configures OpenIddict server for OAuth 2.0 authentication
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOpenIddictServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the authorization, logout, token and userinfo endpoints.
                options
                    .SetAuthorizationEndpointUris("/connect/authorize")
                    .SetEndSessionEndpointUris("/connect/logout")
                    .SetTokenEndpointUris("/connect/token")
                    .SetUserInfoEndpointUris("/connect/userinfo");

                // Enable the authorization code flow.
                options.AllowAuthorizationCodeFlow();
                
                // Enable the client credentials flow.
                options.AllowClientCredentialsFlow();
                
                // Enable the resource owner password credentials flow.
                options.AllowPasswordFlow();
                
                // Enable the refresh token flow.
                options.AllowRefreshTokenFlow();

                // Accept anonymous clients (i.e clients that don't send a client_id).
                options.AcceptAnonymousClients();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableEndSessionEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserInfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration();
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        return services;
    }

    /// <summary>
    /// Seeds initial OAuth clients and scopes
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="configuration">Application settings, including environment variables</param>
    /// <returns>Task</returns>
    public static async Task SeedOpenIddictDataAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var clientId = GetRequiredSetting(configuration, "OpenIddict:Client:Id");

        // Check if the client application already exists
        if (await manager.FindByClientIdAsync(clientId) == null)
        {
            var redirectUris = GetRequiredUris(configuration, "OpenIddict:Client:RedirectUris");
            var postLogoutRedirectUris = GetRequiredUris(configuration, "OpenIddict:Client:PostLogoutRedirectUris");
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = GetRequiredSetting(configuration, "OpenIddict:Client:Secret"),
                ConsentType = ConsentTypes.Implicit,
                DisplayName = GetRequiredSetting(configuration, "OpenIddict:Client:DisplayName"),
                ClientType = ClientTypes.Confidential,
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.EndSession,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                },
            };

            foreach (var uri in redirectUris)
            {
                descriptor.RedirectUris.Add(uri);
            }

            foreach (var uri in postLogoutRedirectUris)
            {
                descriptor.PostLogoutRedirectUris.Add(uri);
            }

            await manager.CreateAsync(descriptor);
        }
    }

    private static string GetRequiredSetting(IConfiguration configuration, string key) =>
        configuration[key] is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"The {key.Replace(':', '_')} environment variable is required.");

    private static Uri[] GetRequiredUris(IConfiguration configuration, string key)
    {
        var values = configuration.GetSection(key).Get<string[]>();
        if (values is not { Length: > 0 })
        {
            throw new InvalidOperationException($"At least one {key.Replace(':', '_')} environment variable is required.");
        }

        return values.Select(value =>
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                throw new InvalidOperationException($"The {key.Replace(':', '_')} values must be absolute URIs.");
            }

            return uri;
        }).ToArray();
    }
}