using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Identity.Users.GetById;

namespace VisitorApp.Application.Features.Identity.Users.GetById;

public class GetByIdUserQueryRequest : IRequestBase<GetByIdUserQueryResponse>
{
    public Guid Id { get; set; }
} 