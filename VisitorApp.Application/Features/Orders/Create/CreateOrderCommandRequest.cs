using VisitorApp.Contract.Features.Orders.Create;

namespace VisitorApp.Application.Features.Orders.Create;

public class CreateOrderCommandRequest : IRequestBase<CreateOrderCommandResponse>
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
