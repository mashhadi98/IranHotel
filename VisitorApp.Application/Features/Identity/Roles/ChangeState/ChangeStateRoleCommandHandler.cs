using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Roles.ChangeState;

namespace VisitorApp.Application.Features.Identity.Roles.ChangeState;

public class ChangeStateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    IEnumerable<IValidatorService<ChangeStateRoleCommandRequest, ChangeStateRoleCommandResponse>> validators) 
    : RequestHandlerBase<ChangeStateRoleCommandRequest, ChangeStateRoleCommandResponse>(validators)
{
    public override async Task<ChangeStateRoleCommandResponse> Handler(ChangeStateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
        {
            throw new ArgumentException(ErrorMessages.Roles.NotFound);
        }

        if (request.IsActive)
        {
            role.ActivateRole();
        }
        else
        {
            role.DeactivateRole();
        }

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to change role state: {errors}");
        }

        var response = mapper.Map<ChangeStateRoleCommandResponse>(role);
        return response;
    }
} 