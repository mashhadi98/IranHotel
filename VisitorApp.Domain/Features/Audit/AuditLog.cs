using VisitorApp.Domain.Common.Entities;

namespace VisitorApp.Domain.Features.Audit;

public class AuditLog : EntityBase<Guid>
{
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Insert|Update|Delete
    public string TableName { get; set; } = string.Empty;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? AffectedColumns { get; set; } // JSON
    public string PrimaryKey { get; set; } = string.Empty; // JSON

    public AuditLog()
    {
        Id = Guid.NewGuid();
    }
}
