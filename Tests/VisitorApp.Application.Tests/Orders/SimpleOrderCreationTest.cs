using Microsoft.EntityFrameworkCore;
using VisitorApp.Domain.Features.Orders.Entities;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Application.Tests.Orders;

public class SimpleOrderCreationTest
{
    [Fact]
    public void AddingOrder_ShouldSetOrderIdOnOrderItems()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var order = new Order
        {
            OrderNumber = "#TEST",
            CustomerId = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        var orderItem = new OrderItem
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 100_000m
        };

        order.Items.Add(orderItem);
        context.Orders.Add(order);

        Assert.Equal(order.Id, orderItem.OrderId);
    }
}
