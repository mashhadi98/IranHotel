using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VisitorApp.Persistence.Common.Context;

/// <summary>
/// Factory for creating ApplicationDbContext instances at design time (migrations, etc.)
/// This allows EF tools to create the context without running the full application
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ApplicationDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set the ConnectionStrings__ApplicationDb environment variable before running EF Core migration commands.");
        }
        
        var migrationsAssembly = typeof(ApplicationDbContext).Assembly.GetName().Name;
        
        optionsBuilder.UseSqlServer(connectionString,
            options => 
            {
                options.MigrationsAssembly(migrationsAssembly);
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
