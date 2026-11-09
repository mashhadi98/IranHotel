using Microsoft.AspNetCore.Identity;
using AutoMapper;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Users.ChangeState;

namespace VisitorApp.Application.Features.Identity.Users.ChangeState;

public class ChangeStateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IEnumerable<IValidatorService<ChangeStateUserCommandRequest, ChangeStateUserCommandResponse>> validators) 
    : RequestHandlerBase<ChangeStateUserCommandRequest, ChangeStateUserCommandResponse>(validators)
{
    public override async Task<ChangeStateUserCommandResponse> Handler(ChangeStateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            throw new ArgumentException(ErrorMessages.Users.NotFound);
        }

        if (request.IsActive)
        {
            user.ActivateUser();
        }
        else
        {
            user.DeactivateUser();
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to change user state: {errors}");
        }

        var response = mapper.Map<ChangeStateUserCommandResponse>(user);
        return response;
    }
} 