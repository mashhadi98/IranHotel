using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Roles.ChangeState;

namespace VisitorApp.Application.Features.Identity.Roles.ChangeState;

public class ChangeStateRoleCommandRequest : IRequestBase<ChangeStateRoleCommandResponse>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
} 