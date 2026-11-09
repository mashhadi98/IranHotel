using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Roles.ChangeState;

public class ChangeStateRoleRequest : RequestBase
{
    public ChangeStateRoleRequest() : base("Roles/{Id}/ChangeState", ApiTypes.Patch) { }

    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
