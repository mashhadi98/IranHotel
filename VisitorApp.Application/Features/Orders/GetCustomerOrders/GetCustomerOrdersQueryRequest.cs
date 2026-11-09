using VisitorApp.Contract.Features.Orders.GetCustomerOrders;

namespace VisitorApp.Application.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersQueryRequest : Pagination, IRequestBase<PaginatedResponse<GetCustomerOrdersQueryResponse>>
{
    public Guid CustomerId { get; set; }
}
