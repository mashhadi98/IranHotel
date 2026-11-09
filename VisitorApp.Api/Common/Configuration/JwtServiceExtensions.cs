using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VisitorApp.Application.Common.Configuration;
using System.Text;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring JWT authentication services
/// </summary>
public static class JwtServiceExtensions
{
    /// <summary>
    /// Configures JWT authentication services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");
        if (string.IsNullOrWhiteSpace(settings.SigningKey) ||
            Encoding.UTF8.GetByteCount(settings.SigningKey) < 32 ||
            string.IsNullOrWhiteSpace(settings.Issuer) ||
            string.IsNullOrWhiteSpace(settings.Audience) ||
            settings.AccessTokenMinutes <= 0 ||
            settings.RefreshTokenDays <= 0 ||
            settings.RequireHttpsMetadata is null)
        {
            throw new InvalidOperationException(
                "JWT settings are invalid. Configure a signing key of at least 32 bytes, issuer, audience, and positive token lifetimes.");
        }

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        var key = Encoding.UTF8.GetBytes(settings.SigningKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = settings.RequireHttpsMetadata.Value;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // Remove default 5 minutes tolerance
                RequireExpirationTime = true
            };
            
            x.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("JWT Token validated successfully");
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
} 