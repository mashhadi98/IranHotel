using VisitorApp.Persistence.Common.Context;
using Serilog.Sinks.MSSqlServer;

namespace VisitorApp.API.Common.Configuration;

/// <summary>
/// Extension methods for configuring logging services
/// </summary>
public static class LoggingServiceExtensions
{
    /// <summary>
    /// Configures Serilog logging services with console and optional SQL Server database logging
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddLoggingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get connection string from environment variable or configuration
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ApplicationDb")
            ?? configuration.GetConnectionString("ApplicationDb");
        
        var loggerConfig = new LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .MinimumLevel.Information();

        // Try to add SQL Server logging with error handling
        if (!string.IsNullOrEmpty(connectionString))
        {
            try
            {
                Console.WriteLine($"Attempting to configure SQL Server logging with connection: {MaskConnectionString(connectionString)}");
                
                // Test the connection first before adding the sink
                if (TestSqlConnection(connectionString))
                {
                    loggerConfig.WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions 
                        { 
                            TableName = "Logs",
                            AutoCreateSqlTable = true,
                            SchemaName = "dbo"
                        });
                    Console.WriteLine("SQL Server logging configured successfully");
                }
                else
                {
                    Console.WriteLine("SQL Server connection test failed. Continuing with console logging only.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to configure SQL Server logging: {ex.Message}");
                Console.WriteLine("Continuing with console logging only.");
            }
        }
        else
        {
            Console.WriteLine("No connection string found. Using console logging only.");
        }

        Log.Logger = loggerConfig.CreateLogger();

        return services;
    }

    /// <summary>
    /// Tests SQL Server connection availability
    /// </summary>
    /// <param name="connectionString">The connection string to test</param>
    /// <returns>True if connection is successful</returns>
    private static bool TestSqlConnection(string connectionString)
    {
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            connection.Open();
            connection.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Masks sensitive information in connection string for logging
    /// </summary>
    /// <param name="connectionString">The connection string to mask</param>
    /// <returns>Masked connection string</returns>
    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "Empty";

        try
        {
            var parts = connectionString.Split(';');
            var maskedParts = parts.Select(part =>
            {
                var lowerPart = part.ToLowerInvariant();
                if (lowerPart.Contains("password") || lowerPart.Contains("pwd"))
                    return part.Split('=')[0] + "=***";
                return part;
            });
            return string.Join(";", maskedParts);
        }
        catch
        {
            return "Unable to parse connection string";
        }
    }
}