using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pluralize.NET;
using VisitorApp.Domain.Features.Audit;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Persistence.Features.Identity.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        var pluralizer = new Pluralizer();
        string tableName = pluralizer.Pluralize(nameof(ApplicationRole));
        builder.ToTable(tableName);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.NormalizedName)
            .HasMaxLength(256);

        builder.Property(e => e.Version)
            .IsConcurrencyToken();

        builder.HasIndex(e => e.NormalizedName)
            .IsUnique();
    }
} 