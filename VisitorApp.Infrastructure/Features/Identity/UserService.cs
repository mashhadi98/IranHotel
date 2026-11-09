using Microsoft.AspNetCore.Identity;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Features.Identity.Interfaces;
using VisitorApp.Domain.Shared;
using VisitorApp.Domain.Common.ResponseMessages;

namespace VisitorApp.Infrastructure.Features.Identity;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<ApplicationUser>> CreateUserAsync(string email, string firstName, string lastName, string? phoneNumber = null, CancellationToken ct = default)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException(ErrorMessages.Users.EmailAlreadyExists);
            }

            // Create new user
            var user = new ApplicationUser(email, firstName, lastName)
            {
                PhoneNumber = phoneNumber,
                EmailConfirmed = true // Auto-confirm for admin created users
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            return Result.Success(user);
        }
        catch (Exception)
        {
            throw; // Re-throw the exception
        }
    }

    public async Task<Result<ApplicationUser>> UpdateUserAsync(Guid userId, string firstName, string lastName, string? phoneNumber = null, CancellationToken ct = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException(ErrorMessages.Users.NotFound);
            }

            user.UpdateProfile(firstName, lastName);
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user: {errors}");
            }

            return Result.Success(user);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException(ErrorMessages.Users.NotFound);
            }

            // Check if user is an admin
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Admin") || userRoles.Contains("admin"))
            {
                throw new InvalidOperationException(ErrorMessages.Users.CannotDeleteAdmin);
            }

            // Soft delete (deactivate user)
            user.DeactivateUser();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to delete user: {errors}");
            }

            return Result.Success();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<ApplicationUser>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException(ErrorMessages.Users.NotFound);
            }

            return Result.Success(user);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result> ActivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException(ErrorMessages.Users.NotFound);
            }

            if (user.IsActive)
            {
                throw new InvalidOperationException(ErrorMessages.Users.UserAlreadyActive);
            }

            user.ActivateUser();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to activate user: {errors}");
            }

            return Result.Success();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result> DeactivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException(ErrorMessages.Users.NotFound);
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException(ErrorMessages.Users.UserAlreadyInactive);
            }

            user.DeactivateUser();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to deactivate user: {errors}");
            }

            return Result.Success();
        }
        catch (Exception)
        {
            throw;
        }
    }
} 