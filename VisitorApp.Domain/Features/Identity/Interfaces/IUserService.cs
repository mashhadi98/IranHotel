using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Shared;

namespace VisitorApp.Domain.Features.Identity.Interfaces;

public interface IUserService
{
    Task<Result<ApplicationUser>> CreateUserAsync(string email, string firstName, string lastName, string? phoneNumber = null, CancellationToken ct = default);
    Task<Result<ApplicationUser>> UpdateUserAsync(Guid userId, string firstName, string lastName, string? phoneNumber = null, CancellationToken ct = default);
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default);
    Task<Result<ApplicationUser>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Task<Result> ActivateUserAsync(Guid userId, CancellationToken ct = default);
    Task<Result> DeactivateUserAsync(Guid userId, CancellationToken ct = default);
} 