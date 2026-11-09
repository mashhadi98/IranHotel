namespace VisitorApp.Domain.Common.Contracts;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
public interface IEntity : IEntity<Guid>
{
}
