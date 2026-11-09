using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Roles.Update;

public class UpdateRoleRequest : RequestBase
{
    public UpdateRoleRequest() : base("Roles/{Id}", ApiTypes.Put) { }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}
