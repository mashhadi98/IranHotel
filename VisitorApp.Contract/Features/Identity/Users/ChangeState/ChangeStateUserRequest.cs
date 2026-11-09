using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Users.ChangeState;

public class ChangeStateUserRequest : RequestBase
{
    public ChangeStateUserRequest() : base("Users/{Id}/ChangeState", ApiTypes.Patch) { }

    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
