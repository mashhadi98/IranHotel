using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Register;

public class RegisterRequest() : RequestBase("Identity/Register", ApiTypes.Post)
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
