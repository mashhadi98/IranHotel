using VisitorApp.Contract.Common;
namespace VisitorApp.Contract.Features.Orders.Create;

public class CreateOrderRequest() : RequestBase("Orders", ApiTypes.Post)
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
