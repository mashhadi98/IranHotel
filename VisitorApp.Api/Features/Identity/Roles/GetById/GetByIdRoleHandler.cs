using VisitorApp.Application.Features.Identity.Roles.GetById;
using VisitorApp.Contract.Features.Identity.Roles.GetById;

namespace VisitorApp.API.Features.Identity.Roles.GetById;

public class GetByIdRoleHandler : GetEndpoint<GetByIdRoleRequest, GetByIdRoleQueryRequest, GetByIdRoleQueryResponse>
{
    public override string? RolesAccess => "Admin";
    
    public GetByIdRoleHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 