using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Roles.Delete;

public class DeleteRoleRequest : RequestBase
{
    public DeleteRoleRequest() : base("Roles/{Id}", ApiTypes.Delete) { }

    public Guid Id { get; set; }
}
