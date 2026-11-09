using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Shared;

namespace VisitorApp.Domain.Features.Identity.Interfaces;

public interface IRoleService
{
    Task<Result<ApplicationRole>> CreateRoleAsync(string name, string description, CancellationToken ct = default);
    Task<Result<ApplicationRole>> UpdateRoleAsync(Guid roleId, string name, string description, CancellationToken ct = default);
    Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken ct = default);
    Task<Result<ApplicationRole>> GetRoleByIdAsync(Guid roleId, CancellationToken ct = default);
    Task<Result> ActivateRoleAsync(Guid roleId, CancellationToken ct = default);
    Task<Result> DeactivateRoleAsync(Guid roleId, CancellationToken ct = default);
} 