using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pluralize.NET;
using VisitorApp.Domain.Features.Audit;

namespace VisitorApp.Persistence.Features.Audit.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        var pluralizer = new Pluralizer();
        string tableName = pluralizer.Pluralize(nameof(AuditLog));
        builder.ToTable(tableName);

        builder.HasKey(d => d.Id);

    }
}