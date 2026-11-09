using VisitorApp.Application.Features.Identity.Roles.GetPaginated;
using VisitorApp.Contract.Features.Identity.Roles.GetPaginated;

namespace VisitorApp.API.Features.Identity.Roles.GetPaginated;

public class GetPaginatedRoleHandler : PaginatedEndpoint<GetPaginatedRoleRequest, GetPaginatedRoleQueryRequest, GetPaginatedRoleQueryResponse>
{
    public override string? RolesAccess => "Admin";
    
    public GetPaginatedRoleHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 