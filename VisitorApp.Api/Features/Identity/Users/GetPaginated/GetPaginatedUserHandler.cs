using VisitorApp.Application.Features.Identity.Users.GetPaginated;
using VisitorApp.Contract.Features.Identity.Users.GetPaginated;

namespace VisitorApp.API.Features.Identity.Users.GetPaginated;

public class GetPaginatedUserHandler : PaginatedEndpoint<GetPaginatedUserRequest, GetPaginatedUserQueryRequest, GetPaginatedUserQueryResponse>
{
    public override string? RolesAccess => "Admin";
    
    public GetPaginatedUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 