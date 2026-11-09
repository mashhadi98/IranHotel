using Microsoft.EntityFrameworkCore;
using VisitorApp.Domain.Features.Orders.Entities;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Application.Tests.Orders;

public class CustomerOrdersSqlTranslationTests
{
    [Fact]
    public void CustomerOrderCursor_TranslatesToBoundedSeekQuery()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(string.Empty)
            .Options;
        using var context = new ApplicationDbContext(options);
        var customerId = Guid.NewGuid();
        var afterCreatedAt = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc);
        var afterId = Guid.NewGuid();

        var sql = context.Orders
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId &&
                (order.CreatedAt < afterCreatedAt ||
                 (order.CreatedAt == afterCreatedAt && order.Id.CompareTo(afterId) < 0)))
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .Select(order => new { order.Id, order.CreatedAt, ItemCount = order.Items.Count })
            .Take(21)
            .ToQueryString();

        Assert.Contains("TOP(@", sql.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("=21;", sql.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ORDER BY", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("[CreatedAt] DESC", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("[Id] DESC", sql, StringComparison.OrdinalIgnoreCase);
    }
}
