using VisitorApp.Application.Features.Identity.Users.Update;
using VisitorApp.Contract.Features.Identity.Users.Update;

namespace VisitorApp.API.Features.Identity.Users.Update;

public class UpdateUserHandler : PutEndpoint<UpdateUserRequest, UpdateUserCommandRequest, UpdateUserCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public UpdateUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 