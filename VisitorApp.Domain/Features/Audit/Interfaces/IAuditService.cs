namespace VisitorApp.Domain.Features.Audit.Interfaces;
public interface IAuditService
{
    Task AddAsync(string entityName, string? entityKey, string action, object? oldValue, object? newValue, CancellationToken cancellationToken = default);
}