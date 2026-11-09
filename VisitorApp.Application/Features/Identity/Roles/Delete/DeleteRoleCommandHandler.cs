using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Roles.Delete;

namespace VisitorApp.Application.Features.Identity.Roles.Delete;

public class DeleteRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IEnumerable<IValidatorService<DeleteRoleCommandRequest, DeleteRoleCommandResponse>> validators) 
    : RequestHandlerBase<DeleteRoleCommandRequest, DeleteRoleCommandResponse>(validators)
{
    public override async Task<DeleteRoleCommandResponse> Handler(DeleteRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
        {
            throw new ArgumentException(ErrorMessages.Roles.NotFound);
        }

        // Check if role is system role (prevent deletion of system roles)
        if (role.Name?.ToLower() == "admin" || role.Name?.ToLower() == "user" || role.Name?.ToLower() == "superadmin")
        {
            throw new InvalidOperationException(ErrorMessages.Roles.CannotDeleteSystemRole);
        }

        // Check if role has users
        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any())
        {
            throw new InvalidOperationException(ErrorMessages.Roles.RoleHasUsers);
        }

        // Soft delete by deactivating role
        role.DeactivateRole();
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to delete role: {errors}");
        }

        return new DeleteRoleCommandResponse { Id = request.Id };
    }
} 