using VisitorApp.Domain.Common.Contracts;

namespace VisitorApp.Domain.Common.Entities;

public abstract class Entity : EntityWithAudit<Guid>, IHasConcurrencyVersion, IEntity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public long Version { get; set; } = 0;
}