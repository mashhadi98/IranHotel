using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Identity.Login;

public class LoginRequest() : RequestBase("Identity/Login", ApiTypes.Post)
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}
