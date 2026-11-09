using VisitorApp.Application.Features.Identity.Users.Delete;
using VisitorApp.Contract.Features.Identity.Users.Delete;

namespace VisitorApp.API.Features.Identity.Users.Delete;

public class DeleteUserHandler : DeleteEndpoint<DeleteUserRequest, DeleteUserCommandRequest, DeleteUserCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public DeleteUserHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
} 