using VisitorApp.Domain.Common.Entities;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Domain.Features.Customers.Entities;

public class Customer : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}