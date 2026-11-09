using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersRequest() : RequestBase("Customers/{CustomerId}/Orders", ApiTypes.Get)
{
    public Guid CustomerId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public DateTime? AfterCreatedAt { get; set; }
    public Guid? AfterId { get; set; }
}
