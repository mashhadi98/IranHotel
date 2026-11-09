using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersRequest() : PaginatedRequestBase("Customers/{CustomerId}/Orders", ApiTypes.Get)
{
    public Guid CustomerId { get; set; }
}
