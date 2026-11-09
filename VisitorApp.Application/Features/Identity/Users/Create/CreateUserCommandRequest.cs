using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Users.Create;

namespace VisitorApp.Application.Features.Identity.Users.Create;

public class CreateUserCommandRequest : IRequestBase<CreateUserCommandResponse>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
} 