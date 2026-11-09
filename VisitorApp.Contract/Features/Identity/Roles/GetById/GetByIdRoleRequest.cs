using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Roles.GetById;

public class GetByIdRoleRequest : RequestBase
{
    public GetByIdRoleRequest() : base("Roles/{Id}", ApiTypes.Get) { }

    public Guid Id { get; set; }
}
