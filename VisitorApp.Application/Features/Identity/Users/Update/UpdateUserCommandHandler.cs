using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Users.Update;

namespace VisitorApp.Application.Features.Identity.Users.Update;

public class UpdateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IEnumerable<IValidatorService<UpdateUserCommandRequest, UpdateUserCommandResponse>> validators) 
    : RequestHandlerBase<UpdateUserCommandRequest, UpdateUserCommandResponse>(validators)
{
    public override async Task<UpdateUserCommandResponse> Handler(UpdateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            throw new ArgumentException(ErrorMessages.Users.NotFound);
        }

        // Update user properties
        user.UpdateProfile(request.FirstName, request.LastName);
        user.PhoneNumber = request.PhoneNumber;

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                user.ActivateUser();
            else
                user.DeactivateUser();
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update user: {errors}");
        }

        // Map to response
        var response = mapper.Map<UpdateUserCommandResponse>(user);
        return response;
    }
} 