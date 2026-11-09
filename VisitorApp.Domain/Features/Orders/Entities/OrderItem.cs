using VisitorApp.Domain.Common.Entities;
using VisitorApp.Domain.Features.Catalog.Entities;

namespace VisitorApp.Domain.Features.Orders.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // قیمت در لحظه سفارش
    public decimal TotalPrice => Quantity * UnitPrice;
}