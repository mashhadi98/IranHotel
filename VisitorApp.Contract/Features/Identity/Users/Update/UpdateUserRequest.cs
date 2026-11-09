using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Users.Update;

public class UpdateUserRequest : RequestBase
{
    public UpdateUserRequest() : base("Users/{Id}", ApiTypes.Put) { }

    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
}
