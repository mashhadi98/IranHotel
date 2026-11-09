using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Roles.Create;

namespace VisitorApp.Application.Features.Identity.Roles.Create;

public class CreateRoleCommandRequest : IRequestBase<CreateRoleCommandResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
} 