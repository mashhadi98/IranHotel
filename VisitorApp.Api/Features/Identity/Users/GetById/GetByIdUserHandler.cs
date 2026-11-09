using VisitorApp.Application.Features.Identity.Users.GetById;
using VisitorApp.Contract.Features.Identity.Users.GetById;

namespace VisitorApp.API.Features.Identity.Users.GetById;

public class GetByIdUserHandler : GetEndpoint<GetByIdUserRequest, GetByIdUserQueryRequest, GetByIdUserQueryResponse>
{
    public override string? RolesAccess => "Admin";
    
    public GetByIdUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 