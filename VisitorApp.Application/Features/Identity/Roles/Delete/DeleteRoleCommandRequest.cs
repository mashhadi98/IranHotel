using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Roles.Delete;

namespace VisitorApp.Application.Features.Identity.Roles.Delete;

public class DeleteRoleCommandRequest : IRequestBase<DeleteRoleCommandResponse>
{
    public Guid Id { get; set; }
} 