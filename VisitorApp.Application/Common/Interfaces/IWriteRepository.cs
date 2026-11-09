using VisitorApp.Domain.Common.Entities;

namespace VisitorApp.Application.Common.Interfaces;

public interface IWriteRepository<T> : IWriteRepository<T, Guid>
    where T : Entity
{

}

public interface IWriteRepository<T, TKey>
    where T : EntityBase<TKey>
{
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, bool autoSave = true, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, bool autoSave = true, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, bool autoSave = true, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, bool autoSave = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, bool autoSave = true, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, bool autoSave = true, CancellationToken cancellationToken = default);
}
