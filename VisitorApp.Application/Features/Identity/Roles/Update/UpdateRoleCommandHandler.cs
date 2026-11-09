using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Roles.Update;

namespace VisitorApp.Application.Features.Identity.Roles.Update;

public class UpdateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    IEnumerable<IValidatorService<UpdateRoleCommandRequest, UpdateRoleCommandResponse>> validators) 
    : RequestHandlerBase<UpdateRoleCommandRequest, UpdateRoleCommandResponse>(validators)
{
    public override async Task<UpdateRoleCommandResponse> Handler(UpdateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
        {
            throw new ArgumentException(ErrorMessages.Roles.NotFound);
        }

        // Check if new name already exists (if name is being changed)
        if (role.Name != request.Name)
        {
            var existingRole = await roleManager.FindByNameAsync(request.Name);
            if (existingRole != null)
            {
                throw new ArgumentException(ErrorMessages.Roles.NameAlreadyExists);
            }
        }

        // Update role properties
        role.UpdateRole(request.Name, request.Description);

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                role.ActivateRole();
            else
                role.DeactivateRole();
        }

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update role: {errors}");
        }

        // Map to response
        var response = mapper.Map<UpdateRoleCommandResponse>(role);
        return response;
    }
} 