using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Services;
using VisitorApp.Contract.Features.Identity.GetCurrentUser;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Application.Features.Identity.GetCurrentUser;

public class GetCurrentUserQueryHandler : RequestHandlerBase<GetCurrentUserQueryRequest, GetCurrentUserQueryResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public override async Task<GetCurrentUserQueryResponse> Handler(GetCurrentUserQueryRequest request, CancellationToken cancellationToken)
    {
        // Get user ID from current user service (extracted from JWT token)
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString()!);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is deactivated");
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        return new GetCurrentUserQueryResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Roles = roles.ToList()
        };
    }
}
