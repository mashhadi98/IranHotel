using VisitorApp.Domain.Common.Entities;

namespace VisitorApp.Application.Common.Interfaces;

public interface IRepository<T, TKey> : IWriteRepository<T, TKey>, IReadRepository<T, TKey>
    where T : EntityBase<TKey>
{
    Task<int> SaveAsync(CancellationToken cancellationToken = default);

}
public interface IRepository<T> : IWriteRepository<T>, IReadRepository<T>
    where T : Entity
{
    Task<int> SaveAsync(CancellationToken cancellationToken = default);

}
