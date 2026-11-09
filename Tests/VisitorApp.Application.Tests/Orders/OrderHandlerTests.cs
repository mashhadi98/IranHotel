using Microsoft.EntityFrameworkCore;
using VisitorApp.Application.Common.Messaging;
using VisitorApp.Application.Common.Services;
using VisitorApp.Application.Features.Orders.ChangeStatus;
using VisitorApp.Application.Features.Orders.Create;
using VisitorApp.Application.Features.Orders.Create.Validators;
using VisitorApp.Application.Common.Interfaces;
using VisitorApp.Contract.Features.Orders.ChangeStatus;
using VisitorApp.Contract.Features.Orders.Create;
using VisitorApp.Domain.Features.Audit.Interfaces;
using VisitorApp.Domain.Features.Catalog.Entities;
using VisitorApp.Domain.Features.Customers.Entities;
using VisitorApp.Domain.Features.Orders.Entities;
using VisitorApp.Infrastructure.Common.Repository;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Application.Tests.Orders;

public class OrderHandlerTests
{
    [Fact]
    public async Task CreateOrder_WithValidRequest_SavesOrderAndDecrementsStock()
    {
        using var database = await OrderTestDatabase.CreateAsync();
        var customer = await database.AddCustomerAsync();
        var product = await database.AddProductAsync(price: 12.50m, stock: 5);
        var handler = database.CreateOrderHandler();

        var result = await handler.Handle(new CreateOrderCommandRequest
        {
            CustomerId = customer.Id,
            Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 2 }]
        }, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(25m, result.TotalAmount);
        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].Quantity);
        Assert.Equal(3, (await database.Context.Products.SingleAsync(
            item => item.Id == product.Id, TestContext.Current.CancellationToken)).Stock);
        Assert.Equal(1, await database.Context.Orders.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateOrder_WithInvalidQuantity_IsRejectedWithoutSaving()
    {
        using var database = await OrderTestDatabase.CreateAsync();
        var customer = await database.AddCustomerAsync();
        var product = await database.AddProductAsync(price: 12.50m, stock: 5);
        var handler = database.CreateOrderHandler();

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(new CreateOrderCommandRequest
        {
            CustomerId = customer.Id,
            Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 0 }]
        }, TestContext.Current.CancellationToken));

        Assert.Equal(0, await database.Context.Orders.CountAsync(TestContext.Current.CancellationToken));
        Assert.Equal(5, (await database.Context.Products.SingleAsync(
            item => item.Id == product.Id, TestContext.Current.CancellationToken)).Stock);
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_IsRejectedWithoutSaving()
    {
        using var database = await OrderTestDatabase.CreateAsync();
        var customer = await database.AddCustomerAsync();
        var product = await database.AddProductAsync(price: 12.50m, stock: 1);
        var handler = database.CreateOrderHandler();

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(new CreateOrderCommandRequest
        {
            CustomerId = customer.Id,
            Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 2 }]
        }, TestContext.Current.CancellationToken));

        Assert.Equal(0, await database.Context.Orders.CountAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, (await database.Context.Products.SingleAsync(
            item => item.Id == product.Id, TestContext.Current.CancellationToken)).Stock);
    }

    [Fact]
    public async Task ChangeOrderStatus_WithAllowedTransition_UpdatesStatus()
    {
        using var database = await OrderTestDatabase.CreateAsync();
        var order = await database.AddOrderAsync(OrderStatus.Pending);
        var handler = database.CreateStatusHandler();

        var result = await handler.Handle(new ChangeOrderStatusCommandRequest
        {
            Id = order.Id,
            Status = nameof(OrderStatus.Confirmed)
        }, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(OrderStatus.Confirmed), result.Status);
        Assert.Equal(OrderStatus.Confirmed, (await database.Context.Orders.SingleAsync(
            item => item.Id == order.Id, TestContext.Current.CancellationToken)).Status);
    }

    [Fact]
    public async Task ChangeOrderStatus_WithDisallowedTransition_IsRejected()
    {
        using var database = await OrderTestDatabase.CreateAsync();
        var order = await database.AddOrderAsync(OrderStatus.Pending);
        var handler = database.CreateStatusHandler();

        await Assert.ThrowsAsync<VisitorApp.Domain.Common.Exceptions.BusinessLogicException>(() =>
            handler.Handle(new ChangeOrderStatusCommandRequest
            {
                Id = order.Id,
                Status = nameof(OrderStatus.Completed)
            }, TestContext.Current.CancellationToken));

        Assert.Equal(OrderStatus.Pending, (await database.Context.Orders.SingleAsync(
            item => item.Id == order.Id, TestContext.Current.CancellationToken)).Status);
    }

    [Fact]
    public async Task ChangeOrderStatus_WhenOrderDoesNotExist_IsRejected()
    {
        using var database = await OrderTestDatabase.CreateAsync();
        var handler = database.CreateStatusHandler();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new ChangeOrderStatusCommandRequest
            {
                Id = Guid.NewGuid(),
                Status = nameof(OrderStatus.Confirmed)
            }, TestContext.Current.CancellationToken));
    }

    private sealed class OrderTestDatabase : IDisposable
    {
        private readonly IAuditService auditService = new NoOpAuditService();
        private readonly ICurrentUserService currentUser = new TestCurrentUserService();

        private OrderTestDatabase(ApplicationDbContext context)
        {
            Context = context;
        }

        public ApplicationDbContext Context { get; }

        public static async Task<OrderTestDatabase> CreateAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"orders-{Guid.NewGuid():N}")
                .Options;
            var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
            return new OrderTestDatabase(context);
        }

        public async Task<Customer> AddCustomerAsync()
        {
            var customer = new Customer { Name = "Test Customer", Email = $"{Guid.NewGuid():N}@example.test" };
            Context.Customers.Add(customer);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            return customer;
        }

        public async Task<Product> AddProductAsync(decimal price, int stock)
        {
            var product = new Product { Name = "Test Product", Price = price, Stock = stock };
            Context.Products.Add(product);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            return product;
        }

        public async Task<Order> AddOrderAsync(OrderStatus status)
        {
            var customer = await AddCustomerAsync();
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderNumber = $"#{Guid.NewGuid():N}",
                Status = status
            };
            Context.Orders.Add(order);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            return order;
        }

        public CreateOrderCommandHandler CreateOrderHandler() =>
            new(
                new EfRepository<Order>(Context, auditService),
                new EfRepository<Customer>(Context, auditService),
                new EfRepository<Product>(Context, auditService),
                currentUser,
                [new CreateOrderCommandValidator()]);

        public ChangeOrderStatusCommandHandler CreateStatusHandler() =>
            new(new EfRepository<Order>(Context, auditService));

        public void Dispose() => Context.Dispose();
    }

    private sealed class NoOpAuditService : IAuditService
    {
        public Task AddAsync(string entityName, string? entityKey, string action, object? oldValue, object? newValue,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => null;
        public string? UserName => "test-user";
    }
}
