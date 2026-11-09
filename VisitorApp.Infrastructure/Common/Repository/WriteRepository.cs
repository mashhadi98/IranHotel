using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Domain.Common.Contracts;
using VisitorApp.Domain.Common.Entities;
using VisitorApp.Domain.Features.Audit.Interfaces;
using VisitorApp.Persistence.Common.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VisitorApp.Infrastructure.Common.Repository;

public class WriteRepository<T> : WriteRepository<T, Guid>, IWriteRepository<T>
    where T : Entity, IHasConcurrencyVersion
{
    public WriteRepository(ApplicationDbContext db, IAuditService auditService) : base(db, auditService)
    {
    }
}

public class WriteRepository<T, TKey> : ReadRepository<T, TKey>, IWriteRepository<T, TKey>
    where T : EntityBase<TKey>, IHasConcurrencyVersion
{

    private readonly IAuditService _auditService;

    public WriteRepository(ApplicationDbContext db, IAuditService auditService) : base(db)
    {
        _auditService = auditService;
    }


    #region Write (with audit)

    public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        // Using EF Find if possible
        // Use object? because TKey can be Guid/int/string
        return await _dbSet.FindAsync(new object?[] { id }, cancellationToken) as T;
    }

    public virtual async Task AddAsync(T entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        // If entity supports audit timestamps, set Created
        TrySetCreated(entity);

        await _dbSet.AddAsync(entity, cancellationToken);
        
        // Temporarily disabled audit service to debug
        // await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "Insert", null, entity, cancellationToken);

        if (autoSave)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        foreach (var e in entities)
        {
            TrySetCreated(e);
        }

        await _dbSet.AddRangeAsync(entities, cancellationToken);
        foreach (var e in entities)
        {
            await _auditService.AddAsync(typeof(T).Name, ExtractKey(e), "Insert", null, e, cancellationToken);
        }

        if (autoSave)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task UpdateAsync(T entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        TrySetUpdated(entity);

        // compute diff if possible (best-effort)
        var diff = ComputeDiff(entity);
        _dbSet.Update(entity);
        await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "Update", null, diff ?? entity, cancellationToken);

        if (autoSave)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        foreach (var e in entities)
        {
            TrySetUpdated(e);
        }
        _dbSet.UpdateRange(entities);
        foreach (var e in entities)
        {
            await _auditService.AddAsync(typeof(T).Name, ExtractKey(e), "Update", null, e, cancellationToken);
        }

        if (autoSave)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task DeleteAsync(T entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        if (entity is VisitorApp.Domain.Common.Contracts.IHardDeletable)
        {
            _dbSet.Remove(entity);
            await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "HardDelete", entity, null, cancellationToken);
        }
        else if (entity is VisitorApp.Domain.Common.Contracts.ISoftDeletable)
        {
            var soft = entity as VisitorApp.Domain.Common.Contracts.ISoftDeletable;
            soft!.SoftDelete();
            _dbSet.Update(entity);
            await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "SoftDelete", null, new { soft!.RemovedAt }, cancellationToken);
        }
        else
        {
            _dbSet.Remove(entity);
            await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "HardDelete", entity, null, cancellationToken);
        }

        if (autoSave)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            if (entity is VisitorApp.Domain.Common.Contracts.IHardDeletable)
            {
                _dbSet.Remove(entity);
                await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "HardDelete", entity, null, cancellationToken);
            }
            else if (entity is VisitorApp.Domain.Common.Contracts.ISoftDeletable)
            {
                var soft = entity as VisitorApp.Domain.Common.Contracts.ISoftDeletable;
                soft!.SoftDelete();
                _dbSet.Update(entity);
                await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "SoftDelete", null, new { soft!.RemovedAt }, cancellationToken);
            }
            else
            {
                _dbSet.Remove(entity);
                await _auditService.AddAsync(typeof(T).Name, ExtractKey(entity), "HardDelete", entity, null, cancellationToken);
            }
        }

        if (autoSave)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Helpers

    protected virtual void TrySetCreated(object entity)
    {
        if (entity is VisitorApp.Domain.Common.Contracts.IAuditable auditable)
        {
            auditable.SetCreated();
        }
    }

    protected virtual void TrySetUpdated(object entity)
    {
        if (entity is VisitorApp.Domain.Common.Contracts.IAuditable auditable)
        {
            auditable.SetUpdated();
        }
    }

    protected virtual string? ExtractKey(object entity)
    {
        // Attempt common keys: Id (Guid/int/long/string)
        var idProp = entity.GetType().GetProperty("Id");
        if (idProp == null)
        {
            return null;
        }
        var val = idProp.GetValue(entity);
        return val?.ToString();
    }

    protected virtual object? ComputeDiff(object entity)
    {
        // Best-effort: try to get tracked Entry and gather modified properties.
        try
        {
            var entry = _db.Entry(entity);
            if (entry == null) return null;

            var diff = new Dictionary<string, object?>();
            foreach (var p in entry.Properties)
            {
                if (!p.IsModified) continue;
                diff[p.Metadata.Name] = new
                {
                    Old = p.OriginalValue,
                    New = p.CurrentValue
                };
            }

            return diff.Count > 0 ? diff : null;
        }
        catch
        {
            return null;
        }
    }

    #endregion


}
