using VisitorApp.Domain.Common.Entities;

namespace VisitorApp.Application.Common.Interfaces;

public interface IReadRepository<T> : IReadRepository<T, Guid>
    where T : Entity
{

}
public interface IReadRepository<T, TKey>
    where T : EntityBase<TKey>
{
    IQueryable<T> GetQuery();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TResult?> GetByFieldAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<List<TResult>> GetListAsync<TResult>(Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default, bool ignoreQueryFilters = false);
    Task<List<TResult>> GetListAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default, SortingBy? sort = null, bool ignoreQueryFilters = false);
    Task<PaginatedResponse<TResult>> GetListPaginatedAsync<TResult, TFilter>(Pagination<TFilter> pagination, Expression<Func<T, bool>>? predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TResult>> GetListPaginatedAsync<TResult>(Pagination pagination, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TResult>> GetListPaginatedAsync<TResult>(Pagination pagination, Expression<Func<T, bool>>? predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<List<DropDownDto>> GetDropDownAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, DropDownDto>> select, CancellationToken cancellationToken = default);
    Task<List<DropDownDto>> GetDropDownAsync(Expression<Func<T, DropDownDto>> select, CancellationToken cancellationToken = default);
    Task<List<TResult>> GetDropDownAsync<TResult>(Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<List<TResult>> GetDropDownAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
}
