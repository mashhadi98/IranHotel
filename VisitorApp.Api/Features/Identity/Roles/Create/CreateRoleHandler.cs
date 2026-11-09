using VisitorApp.Application.Features.Identity.Roles.Create;
using VisitorApp.Contract.Features.Identity.Roles.Create;

namespace VisitorApp.API.Features.Identity.Roles.Create;

public class CreateRoleHandler : PostEndpoint<CreateRoleRequest, CreateRoleCommandRequest, CreateRoleCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public CreateRoleHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 