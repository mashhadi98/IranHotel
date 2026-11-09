using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Register;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Application.Features.Identity.Register;

public class RegisterCommandHandler : RequestHandlerBase<RegisterCommandRequest, RegisterCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<RegisterCommandResponse> Handler(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new RegisterCommandResponse
                {
                    IsSuccess = false,
                    Message = "A user with this email already exists"
                };
            }

            // Create new user
            var user = new ApplicationUser(request.Email, request.FirstName, request.LastName);

            // Create user with password
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new RegisterCommandResponse
                {
                    IsSuccess = false,
                    Message = $"Registration failed: {errors}"
                };
            }

            // Create successful response
            return new RegisterCommandResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsSuccess = true,
                Message = "User registered successfully"
            };
        }
        catch (Exception ex)
        {
            return new RegisterCommandResponse
            {
                IsSuccess = false,
                Message = $"An error occurred during registration: {ex.Message}"
            };
        }
    }
} 