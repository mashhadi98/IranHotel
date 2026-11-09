using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Shared;

namespace VisitorApp.Domain.Features.Identity.Interfaces;

public interface IAuthService
{
    Task<Result<ApplicationUser>> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<Result<ApplicationUser>> LoginAsync(string email, string password);
    Task<Result> LogoutAsync(string userId);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<Result<ApplicationUser>> GetUserByIdAsync(string userId);
    Task<Result<ApplicationUser>> GetUserByEmailAsync(string email);
    Task<Result> UpdateUserProfileAsync(string userId, string firstName, string lastName);
    Task<Result> DeactivateUserAsync(string userId);
    Task<Result> ActivateUserAsync(string userId);
} 