using VisitorApp.Application.Features.Identity.Users.Create;
using VisitorApp.Contract.Features.Identity.Users.Create;

namespace VisitorApp.API.Features.Identity.Users.Create;

public class CreateUserHandler : PostEndpoint<CreateUserRequest, CreateUserCommandRequest, CreateUserCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public CreateUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 