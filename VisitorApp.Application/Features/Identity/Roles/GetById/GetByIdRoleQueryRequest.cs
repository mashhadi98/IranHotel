using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Roles.GetById;

namespace VisitorApp.Application.Features.Identity.Roles.GetById;

public class GetByIdRoleQueryRequest : IRequestBase<GetByIdRoleQueryResponse>
{
    public Guid Id { get; set; }
} 