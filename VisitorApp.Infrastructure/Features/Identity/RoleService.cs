using Microsoft.AspNetCore.Identity;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Features.Identity.Interfaces;
using VisitorApp.Domain.Shared;
using VisitorApp.Domain.Common.ResponseMessages;

namespace VisitorApp.Infrastructure.Features.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Result<ApplicationRole>> CreateRoleAsync(string name, string description, CancellationToken ct = default)
    {
        try
        {
            // Check if role already exists
            var existingRole = await _roleManager.FindByNameAsync(name);
            if (existingRole != null)
            {
                throw new InvalidOperationException(ErrorMessages.Roles.NameAlreadyExists);
            }

            var role = new ApplicationRole(name, description);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create role: {errors}");
            }

            return Result.Success(role);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<ApplicationRole>> UpdateRoleAsync(Guid roleId, string name, string description, CancellationToken ct = default)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ArgumentException(ErrorMessages.Roles.NotFound);
            }

            // Check if new name conflicts with existing role
            if (role.Name != name)
            {
                var existingRole = await _roleManager.FindByNameAsync(name);
                if (existingRole != null && existingRole.Id != roleId)
                {
                    throw new InvalidOperationException(ErrorMessages.Roles.NameAlreadyExists);
                }
            }

            role.UpdateRole(name, description);
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update role: {errors}");
            }

            return Result.Success(role);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ArgumentException(ErrorMessages.Roles.NotFound);
            }

            // Check if it's a system role
            if (role.Name?.ToLower() == "admin" || role.Name?.ToLower() == "user" || role.Name?.ToLower() == "superadmin")
            {
                throw new InvalidOperationException(ErrorMessages.Roles.CannotDeleteSystemRole);
            }

            // Check if role has users
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                throw new InvalidOperationException(ErrorMessages.Roles.RoleHasUsers);
            }

            // Soft delete (deactivate role)
            role.DeactivateRole();
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to delete role: {errors}");
            }

            return Result.Success();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<ApplicationRole>> GetRoleByIdAsync(Guid roleId, CancellationToken ct = default)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ArgumentException(ErrorMessages.Roles.NotFound);
            }

            return Result.Success(role);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result> ActivateRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ArgumentException(ErrorMessages.Roles.NotFound);
            }

            if (role.IsActive)
            {
                throw new InvalidOperationException(ErrorMessages.Roles.RoleAlreadyActive);
            }

            role.ActivateRole();
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to activate role: {errors}");
            }

            return Result.Success();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result> DeactivateRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ArgumentException(ErrorMessages.Roles.NotFound);
            }

            if (!role.IsActive)
            {
                throw new InvalidOperationException(ErrorMessages.Roles.RoleAlreadyInactive);
            }

            role.DeactivateRole();
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to deactivate role: {errors}");
            }

            return Result.Success();
        }
        catch (Exception)
        {
            throw;
        }
    }
} 