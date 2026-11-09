using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Login;

namespace VisitorApp.Application.Features.Identity.Login;

public class LoginCommandRequest : IRequestBase<LoginCommandResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
} 