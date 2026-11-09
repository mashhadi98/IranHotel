using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Domain.Common.DTOs;
using VisitorApp.Domain.Common.Entities;
using VisitorApp.Infrastructure.Common.Helpers;
using VisitorApp.Persistence.Common.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace VisitorApp.Infrastructure.Common.Repository;

public class ReadRepository<T> : ReadRepository<T, Guid>, IReadRepository<T>
    where T : Entity
{
    public ReadRepository(ApplicationDbContext db) : base(db)
    {
    }
}

public class ReadRepository<T, TKey> : IReadRepository<T, TKey>
    where T : EntityBase<TKey>
{

    protected readonly ApplicationDbContext _db;
    protected readonly DbSet<T> _dbSet;

    public ReadRepository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }



    public IQueryable<T> GetQuery() => _dbSet.AsQueryable();

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<TResult?> GetByFieldAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).Select(select).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<List<TResult>> GetListAsync<TResult>(Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default, bool ignoreQueryFilters = false)
    {
        return await (ignoreQueryFilters ? _dbSet.IgnoreQueryFilters().Select(select) : _dbSet.Select(select)).ToListAsync(cancellationToken);
    }
    public async Task<List<TResult>> GetListAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default, SortingBy? sort = null, bool ignoreQueryFilters = false)
    {
        var query = ignoreQueryFilters ? _dbSet.IgnoreQueryFilters().Where(predicate) : _dbSet.Where(predicate);

        query = sort switch
        {
            SortingBy.CreateDate => query.OrderByDynamic("CreatedAt", ascending: true),
            SortingBy.DescendingCreateDate => query.OrderByDynamic("CreatedAt", ascending: false),
            _ => query
        };

        return await query.Select(select).ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<TResult>> GetListPaginatedAsync<TResult, TFilter>(Pagination<TFilter> pagination, Expression<Func<T, bool>>? predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        var query = predicate is not null ? _dbSet.Where(predicate) : _dbSet.AsQueryable();

        query = pagination.Sort switch
        {
            SortingBy.CreateDate => query.OrderByDynamic("CreatedAt", ascending: true),
            SortingBy.DescendingCreateDate => query.OrderByDynamic("CreatedAt", ascending: false),
            _ => query
        };

        var total = await query.CountAsync(cancellationToken);
        var list = await query
            .Skip(((pagination.Page ?? 1) - 1) * (pagination.PageSize ?? 10))
            .Take(pagination.PageSize ?? 10)
            .Select(select)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TResult>(list, total);
    }

    public async Task<PaginatedResponse<TResult>> GetListPaginatedAsync<TResult>(Pagination pagination, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await GetListPaginatedAsync<TResult, object?>(new Pagination<object?> { Page = pagination.Page, PageSize = pagination.PageSize, Sort = SortingBy.DescendingCreateDate }, null, select, cancellationToken);
    }
    public async Task<PaginatedResponse<TResult>> GetListPaginatedAsync<TResult>(Pagination pagination, Expression<Func<T, bool>>? predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await GetListPaginatedAsync<TResult, object?>(new Pagination<object?> { Page = pagination.Page, PageSize = pagination.PageSize, Sort = SortingBy.DescendingCreateDate }, predicate, select, cancellationToken);
    }
    public async Task<List<DropDownDto>> GetDropDownAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, DropDownDto>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).Select(select).ToListAsync(cancellationToken);
    }
    public async Task<List<DropDownDto>> GetDropDownAsync(Expression<Func<T, DropDownDto>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Select(select).ToListAsync(cancellationToken);
    }
    public async Task<List<TResult>> GetDropDownAsync<TResult>(Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Select(select).ToListAsync(cancellationToken);
    }
    public async Task<List<TResult>> GetDropDownAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).Select(select).ToListAsync(cancellationToken);
    }
}
