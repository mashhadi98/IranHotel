using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pluralize.NET;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Persistence.Features.Identity.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var pluralizer = new Pluralizer();
        string tableName = pluralizer.Pluralize(nameof(ApplicationUser));
        builder.ToTable(tableName);

        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.UserName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Version)
            .IsConcurrencyToken();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasIndex(e => e.UserName)
            .IsUnique();
    }
}