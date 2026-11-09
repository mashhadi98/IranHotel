using VisitorApp.Contract.Features.Orders.GetById;

namespace VisitorApp.Application.Features.Orders.GetById;

public class GetByIdOrderQueryRequest : IRequestBase<GetByIdOrderQueryResponse>
{
    public Guid Id { get; set; }
}
