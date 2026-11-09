using VisitorApp.Contract.Features.Orders.GetCustomerOrders;

namespace VisitorApp.Application.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersQueryRequest : IRequestBase<CursorPaginatedResponse<GetCustomerOrdersQueryResponse>>
{
    public Guid CustomerId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public DateTime? AfterCreatedAt { get; set; }
    public Guid? AfterId { get; set; }
}
