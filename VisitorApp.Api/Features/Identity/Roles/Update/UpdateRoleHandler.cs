using VisitorApp.Application.Features.Identity.Roles.Update;
using VisitorApp.Contract.Features.Identity.Roles.Update;

namespace VisitorApp.API.Features.Identity.Roles.Update;

public class UpdateRoleHandler : PutEndpoint<UpdateRoleRequest, UpdateRoleCommandRequest, UpdateRoleCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public UpdateRoleHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 