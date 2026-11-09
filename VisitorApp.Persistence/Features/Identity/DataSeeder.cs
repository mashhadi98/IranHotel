using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Persistence.Features.Identity;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(DataSeeder));

        try
        {
            await SeedRolesAsync(roleManager, logger);
            await SeedAdminUserAsync(
                userManager,
                logger,
                GetRequiredSetting(configuration, "AdminSeed:Email"),
                GetRequiredSetting(configuration, "AdminSeed:Password"),
                GetRequiredSetting(configuration, "AdminSeed:FirstName"),
                GetRequiredSetting(configuration, "AdminSeed:LastName"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role"
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role '{RoleName}': {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string adminEmail,
        string adminPassword,
        string firstName,
        string lastName)
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Version = DateTimeOffset.UtcNow.Ticks
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Default admin user created successfully");
                logger.LogInformation("Admin user created: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create admin user: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists");
        }
    }

    private static string GetRequiredSetting(IConfiguration configuration, string key) =>
        configuration[key] is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"The {key.Replace(':', '_')} environment variable is required.");
}