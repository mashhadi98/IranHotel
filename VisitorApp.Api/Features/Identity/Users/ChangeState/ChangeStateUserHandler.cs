using VisitorApp.Application.Features.Identity.Users.ChangeState;
using VisitorApp.Contract.Features.Identity.Users.ChangeState;

namespace VisitorApp.API.Features.Identity.Users.ChangeState;

public class ChangeStateUserHandler : PatchEndpoint<ChangeStateUserRequest, ChangeStateUserCommandRequest, ChangeStateUserCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public ChangeStateUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 