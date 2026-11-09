using Microsoft.AspNetCore.Identity;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Domain.Features.Identity.Entities;
using VisitorApp.Domain.Common.ResponseMessages;
using VisitorApp.Contract.Features.Identity.Roles.Create;

namespace VisitorApp.Application.Features.Identity.Roles.Create;

public class CreateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    IEnumerable<IValidatorService<CreateRoleCommandRequest, CreateRoleCommandResponse>> validators) 
    : RequestHandlerBase<CreateRoleCommandRequest, CreateRoleCommandResponse>(validators)
{
    public override async Task<CreateRoleCommandResponse> Handler(CreateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        // Check if role name already exists
        var existingRole = await roleManager.FindByNameAsync(request.Name);
        if (existingRole != null)
        {
            throw new ArgumentException(ErrorMessages.Roles.NameAlreadyExists);
        }

        // Create new role
        var role = new ApplicationRole(request.Name, request.Description)
        {
            IsActive = request.IsActive
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create role: {errors}");
        }

        // Map to response
        var response = mapper.Map<CreateRoleCommandResponse>(role);
        return response;
    }
} 