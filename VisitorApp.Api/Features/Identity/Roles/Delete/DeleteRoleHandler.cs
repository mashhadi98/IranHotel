using VisitorApp.Application.Features.Identity.Roles.Delete;
using VisitorApp.Contract.Features.Identity.Roles.Delete;

namespace VisitorApp.API.Features.Identity.Roles.Delete;

public class DeleteRoleHandler : DeleteEndpoint<DeleteRoleRequest, DeleteRoleCommandRequest, DeleteRoleCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public DeleteRoleHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 