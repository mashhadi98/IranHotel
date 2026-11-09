using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Users.GetById;

public class GetByIdUserRequest : RequestBase
{
    public GetByIdUserRequest() : base("Users/{Id}", ApiTypes.Get) { }

    public Guid Id { get; set; }
}
