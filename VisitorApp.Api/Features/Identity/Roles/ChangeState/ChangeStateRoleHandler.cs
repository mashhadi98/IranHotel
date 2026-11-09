using VisitorApp.Application.Features.Identity.Roles.ChangeState;
using VisitorApp.Contract.Features.Identity.Roles.ChangeState;

namespace VisitorApp.API.Features.Identity.Roles.ChangeState;

public class ChangeStateRoleHandler : PatchEndpoint<ChangeStateRoleRequest, ChangeStateRoleCommandRequest, ChangeStateRoleCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public ChangeStateRoleHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 