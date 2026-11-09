using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Roles.Create;

public class CreateRoleRequest : RequestBase
{
    public CreateRoleRequest() : base("Roles", ApiTypes.Post) { }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
