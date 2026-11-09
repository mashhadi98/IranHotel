using VisitorApp.Domain.Common.Entities;
using VisitorApp.Domain.Features.Customers.Entities;
using VisitorApp.Domain.Features.Identity.Entities;

namespace VisitorApp.Domain.Features.Orders.Entities;

public class Order : Entity
{
    public string OrderNumber { get; set; } = default!; // مثل #12345
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    
    public Guid? UserId { get; set; } // کاربر ثبت کننده سفارش
    public ApplicationUser? User { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}