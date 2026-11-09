using VisitorApp.Domain.Common.Contracts;

namespace VisitorApp.Domain.Common.Entities;

public abstract class EntityBase<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}