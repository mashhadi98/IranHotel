using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Users.Delete;

namespace VisitorApp.Application.Features.Identity.Users.Delete;

public class DeleteUserCommandRequest : IRequestBase<DeleteUserCommandResponse>
{
    public Guid Id { get; set; }
} 