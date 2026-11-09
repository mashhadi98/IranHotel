using VisitorApp.Persistence.Common.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring SQL Server database services with retry resilience
/// </summary>
public static class DatabaseServiceExtensions
{
	/// <summary>
	/// Configures SQL Server database context with retry resilience and connection timeout settings
	/// - Enables automatic retry on transient failures (up to 5 retries with 30-second max delay)
	/// - Sets command timeout to 60 seconds
	/// - Enables detailed logging in development environment
	/// </summary>
	/// <param name="services">The service collection</param>
	/// <param name="configuration">The configuration instance</param>
	/// <returns>The service collection for chaining</returns>
	public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
	{
		// Get connection string from configuration or environment variable
		var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ApplicationDb")
			?? configuration.GetConnectionString("ApplicationDb");
		if (string.IsNullOrEmpty(connectionString))
		{
			throw new InvalidOperationException("ApplicationDb connection string is not configured. Please set it in appsettings.json or environment variables.");
		}

		var migrationsAssembly = VisitorApp.Persistence.AssemblyReference.Assembly.GetName().Name;

		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(connectionString,
				op => 
				{
					op.MigrationsAssembly(migrationsAssembly);
					
					// Enable retry on failure for transient error resilience
					op.EnableRetryOnFailure(
						maxRetryCount: 5,
						maxRetryDelay: TimeSpan.FromSeconds(30),
						errorNumbersToAdd: null);
					
					// Additional connection resilience settings
					op.CommandTimeout(60); // 60 seconds command timeout
				});
			
			// Enable sensitive data logging in development
			if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
			{
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
		});

		return services;
	}
	
	public static async Task<IApplicationBuilder> AddDatabaseApplication(this IApplicationBuilder app, IConfiguration configuration)
	{
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                logger.LogInformation("Starting database initialization...");
                
                // Get and log connection string (without sensitive data)
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ApplicationDb")
                    ?? configuration.GetConnectionString("ApplicationDb");
                
                if (!string.IsNullOrEmpty(connectionString))
                {
                    // Log connection info without password
                    var safeConnectionString = connectionString.Split(';')
                        .Where(part => !part.ToLowerInvariant().Contains("password") && 
                                      !part.ToLowerInvariant().Contains("pwd"))
                        .Aggregate((a, b) => a + ";" + b);
                    logger.LogInformation("Using connection string: {ConnectionString}", safeConnectionString);
                }
                
                // Test database connection first
                logger.LogInformation("Testing database connection...");
                var canConnect = await applicationDbContext.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    logger.LogError("Cannot connect to database. Please check:");
                    logger.LogError("1. SQL Server service is running");
                    logger.LogError("2. Server name/instance is correct: {ServerInfo}", ExtractServerInfo(connectionString));
                    logger.LogError("3. Database permissions are configured properly");
                    logger.LogError("4. Connection string is valid");
                    throw new InvalidOperationException("Database connection failed. Please check SQL Server configuration.");
                }
                
                logger.LogInformation("Database connection successful!");
                
                // Apply pending migrations (this will create database if it doesn't exist)
                var pendingMigrations = await applicationDbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
                    await applicationDbContext.Database.MigrateAsync();
                    logger.LogInformation("Database migration completed successfully");
                }
                else
                {
                    logger.LogInformation("Database is up to date, no migrations needed");
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                logger.LogError(sqlEx, "SQL Server connection error occurred:");
                logger.LogError("Error Number: {ErrorNumber}", sqlEx.Number);
                logger.LogError("Error Message: {ErrorMessage}", sqlEx.Message);
                
                // Provide specific guidance based on error number
                switch (sqlEx.Number)
                {
                    case 2: // Named pipe provider error
                    case 26: // Error locating server/instance
                        logger.LogError("SQL Server instance not found. Please:");
                        logger.LogError("1. Verify SQL Server service is running");
                        logger.LogError("2. Check server name and instance in connection string");
                        logger.LogError("3. Ensure SQL Server is configured for TCP/IP connections");
                        break;
                    case 18456: // Login failed
                        logger.LogError("Authentication failed. Please:");
                        logger.LogError("1. Check username/password if using SQL authentication");
                        logger.LogError("2. Verify Windows authentication is working");
                        logger.LogError("3. Ensure the user has proper database permissions");
                        break;
                    case 4060: // Invalid database name
                        logger.LogError("Database name issue. The database will be created automatically during migration.");
                        break;
                }
                
                throw new InvalidOperationException($"Database connection failed with SQL Server error {sqlEx.Number}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred during database initialization");
                throw;
            }
        }
        return app;
	}

	/// <summary>
	/// Checks if SQL Server provider is configured by detecting SQL Server-specific connection string patterns
	/// </summary>
	/// <param name="configuration">The configuration instance</param>
	/// <returns>True if SQL Server provider should be used</returns>
	public static bool IsSqlServerConfigured(IConfiguration configuration)
	{
		// Try environment variable first
		var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ApplicationDb");
		
		// Fallback to configuration
		if (string.IsNullOrEmpty(connectionString))
		{
			connectionString = configuration.GetConnectionString("ApplicationDb");
		}
		
		if (string.IsNullOrEmpty(connectionString))
			return false;

		// Check for SQL Server-specific keywords in connection string
		var lowerConnectionString = connectionString.ToLowerInvariant();
		return lowerConnectionString.Contains("server=") || 
			   lowerConnectionString.Contains("data source=") ||
			   lowerConnectionString.Contains("initial catalog=") ||
			   lowerConnectionString.Contains("database=") ||
			   lowerConnectionString.Contains("integrated security=") ||
			   lowerConnectionString.Contains("sqlserver");
	}

	/// <summary>
	/// Extracts server information from connection string for logging purposes
	/// </summary>
	/// <param name="connectionString">The connection string</param>
	/// <returns>Server information string</returns>
	private static string ExtractServerInfo(string? connectionString)
	{
		if (string.IsNullOrEmpty(connectionString))
			return "Unknown";

		try
		{
			var parts = connectionString.Split(';');
			var serverPart = parts.FirstOrDefault(p => 
				p.ToLowerInvariant().StartsWith("server=") || 
				p.ToLowerInvariant().StartsWith("data source="));
				
			if (serverPart != null)
			{
				return serverPart.Split('=')[1];
			}
		}
		catch
		{
			// If parsing fails, just return a safe message
		}
		
		return "Unable to parse server info";
	}
} 