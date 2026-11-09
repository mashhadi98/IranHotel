using VisitorApp.Application.Common.Services;
using VisitorApp.Domain.Features.Audit;
using VisitorApp.Domain.Features.Audit.Interfaces;
using VisitorApp.Persistence.Common.Context;
using System.Text.Json;

namespace VisitorApp.Infrastructure.Features.Audit.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AuditService(ApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task AddAsync(string entityName, string? entityKey, string action, object? oldValue, object? newValue, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            TableName = entityName,
            PrimaryKey = entityKey ?? string.Empty,
            Type = action,
            OldValues = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
            NewValues = newValue != null ? JsonSerializer.Serialize(newValue) : null,
            AffectedColumns = JsonSerializer.Serialize(GetAffectedColumns(oldValue, newValue)),
            DateTime = DateTime.UtcNow,
            UserId = _currentUser.UserId?.ToString() ?? string.Empty
        };

        await _db.AuditLogs.AddAsync(log, cancellationToken);

        // Note: don't save here — UnitOfWork will decide when to SaveChanges.
    }

    private static string[] GetAffectedColumns(object? oldValue, object? newValue)
    {
        var columns = new List<string>();
        
        if (oldValue != null && newValue != null)
        {
            var oldProps = oldValue.GetType().GetProperties();
            var newProps = newValue.GetType().GetProperties();
            
            foreach (var prop in oldProps)
            {
                var newProp = newProps.FirstOrDefault(p => p.Name == prop.Name);
                if (newProp != null)
                {
                    var oldVal = prop.GetValue(oldValue);
                    var newVal = newProp.GetValue(newValue);
                    
                    if (!Equals(oldVal, newVal))
                    {
                        columns.Add(prop.Name);
                    }
                }
            }
        }
        
        return columns.ToArray();
    }
}