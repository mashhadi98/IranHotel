using VisitorApp.Domain.Common.Contracts;

namespace VisitorApp.Domain.Common.Entities;

public abstract class EntityWithAudit<TKey> : EntityBase<TKey>, IAuditable, ISoftDeletable
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? RemovedAt { get; set; }

    public void SetCreated()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void SetUpdated() => UpdatedAt = DateTime.UtcNow;

    public void SoftDelete() => RemovedAt = DateTime.UtcNow;
    public void Restore() => RemovedAt = null;
}