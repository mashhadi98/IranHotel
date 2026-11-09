using VisitorApp.Domain.Common.Contracts;
using VisitorApp.Domain.Features.Audit;
using VisitorApp.Domain.Features.Catalog.Entities;
using VisitorApp.Domain.Features.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VisitorApp.Domain.Features.Customers.Entities;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Persistence.Common.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<OrderItem> OrderItems { get; set; } = default!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = default!;


    public override int SaveChanges()
    {
        ApplyConcurrencyVersionBump();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyConcurrencyVersionBump();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    private void ApplyConcurrencyVersionBump()
    {
        foreach (var entry in ChangeTracker.Entries<IHasConcurrencyVersion>())
        {
            if (entry.State == EntityState.Modified)
            {
                // Versionی که کلاینت فرستاده (قدیمیِ خودش)
                var prop = entry.Property(e => e.Version);
                var clientVersion = prop.CurrentValue;

                // شرط WHERE: Version = @clientVersion
                prop.OriginalValue = clientVersion;

                // SET Version = clientVersion + 1
                prop.CurrentValue = clientVersion + 1;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // حذف هم با شرط Version انجام شود
                var prop = entry.Property(e => e.Version);
                prop.OriginalValue = prop.CurrentValue;
            }
            // EntityState.Added نیاز به کاری ندارد (Version = 0 کافی است)
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var et in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IHasConcurrencyVersion).IsAssignableFrom(et.ClrType))
            {
                modelBuilder.Entity(et.ClrType)
                    .Property<long>(nameof(IHasConcurrencyVersion.Version));
                    //.IsConcurrencyToken();
            }
        }

        // Configure OpenIddict entities
        modelBuilder.UseOpenIddict();

        // Now use the fixed configuration classes
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        base.OnModelCreating(modelBuilder);
    }
} 