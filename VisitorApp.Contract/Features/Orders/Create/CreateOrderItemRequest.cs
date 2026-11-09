namespace VisitorApp.Contract.Features.Orders.Create;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
