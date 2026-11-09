using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Users.Create;

namespace VisitorApp.Application.Features.Identity.Users.Create;

public class CreateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IEnumerable<IValidatorService<CreateUserCommandRequest, CreateUserCommandResponse>> validators) 
    : RequestHandlerBase<CreateUserCommandRequest, CreateUserCommandResponse>(validators)
{
    public override async Task<CreateUserCommandResponse> Handler(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ArgumentException(ErrorMessages.Users.EmailAlreadyExists);
        }

        // Create new user
        var user = new ApplicationUser(request.Email, request.FirstName, request.LastName)
        {
            IsActive = request.IsActive,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true // Auto-confirm for admin created users
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        // Map to response
        var response = mapper.Map<CreateUserCommandResponse>(user);
        return response;
    }
} 