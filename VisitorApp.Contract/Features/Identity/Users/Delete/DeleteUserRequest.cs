using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Users.Delete;

public class DeleteUserRequest : RequestBase
{
    public DeleteUserRequest() : base("Users/{Id}", ApiTypes.Delete) { }

    public Guid Id { get; set; }
}
