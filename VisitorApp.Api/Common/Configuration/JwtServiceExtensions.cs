using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
        // JWT settings - should match LoginCommandHandler
        const string secretKey = "VisitorApp-Super-Secret-Key-For-JWT-Token-Generation-2024-MinLength32Chars";
        const string issuer = "VisitorApp";
        const string audience = "VisitorApp-API";
        
        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false; // Set to true in production
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
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