using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Users.Delete;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Application.Features.Identity.Users.Delete;

public class DeleteUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEnumerable<IValidatorService<DeleteUserCommandRequest, DeleteUserCommandResponse>> validators)
    : RequestHandlerBase<DeleteUserCommandRequest, DeleteUserCommandResponse>(validators)
{
    public override async Task<DeleteUserCommandResponse> Handler(DeleteUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            throw new ArgumentException(ErrorMessages.Users.NotFound);
        }

        // Check if user is admin (prevent deletion of admin users)
        var userRoles = await userManager.GetRolesAsync(user);
        if (userRoles.Contains("Admin") || userRoles.Contains("admin"))
        {
            throw new InvalidOperationException(ErrorMessages.Users.CannotDeleteAdmin);
        }

        // Soft delete by deactivating user
        user.DeactivateUser();
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to delete user: {errors}");
        }

        return new DeleteUserCommandResponse { Id = request.Id };
    }
}