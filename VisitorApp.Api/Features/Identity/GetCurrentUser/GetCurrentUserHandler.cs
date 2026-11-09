using VisitorApp.Contract.Features.Identity.GetCurrentUser;

namespace VisitorApp.API.Features.Identity.GetCurrentUser;

public class GetCurrentUserHandler : GetEndpointWithoutRequest<GetCurrentUserRequest, GetCurrentUserQueryRequest, GetCurrentUserQueryResponse>
{
    public override string? RolesAccess => null; // همه کاربران احراز هویت شده

    public GetCurrentUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender)
    {
    }
}
