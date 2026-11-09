using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Domain.Common.Contracts;
using VisitorApp.Domain.Common.Entities;
using VisitorApp.Domain.Features.Audit.Interfaces;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Infrastructure.Common.Repository;

public class EfRepository<T> : EfRepository<T, Guid>, IRepository<T>
    where T : Entity
{
    public EfRepository(ApplicationDbContext db, IAuditService auditService) : base(db, auditService)
    {
    }
}
public class EfRepository<T, TKey> : WriteRepository<T, TKey>, IRepository<T, TKey>
    where T : EntityBase<TKey>, IHasConcurrencyVersion
{
    public EfRepository(ApplicationDbContext db, IAuditService auditService) : base(db, auditService)
    {
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }
}
