using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Register;

namespace VisitorApp.Application.Features.Identity.Register;

public class RegisterCommandRequest : IRequestBase<RegisterCommandResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
} 