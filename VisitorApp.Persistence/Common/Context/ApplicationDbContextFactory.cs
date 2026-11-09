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
        
        // Connection string for migrations
        // Note: This is only used during design-time (migrations)
        var connectionString = "Data Source=DESKTOP-VV19P79\\SQL2025;Initial Catalog=IranhotelDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Command Timeout=0";
        
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
