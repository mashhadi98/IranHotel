using Microsoft.EntityFrameworkCore;
using VisitorApp.Application.Common.Services;
using VisitorApp.Contract.Features.Orders.Common;
using VisitorApp.Contract.Features.Orders.Create;
using VisitorApp.Domain.Features.Customers.Entities;
using VisitorApp.Domain.Features.Orders.Entities;
using VisitorApp.Domain.Features.Catalog.Entities;
using VisitorApp.Application.Common.Messaging;

namespace VisitorApp.Application.Features.Orders.Create;

public class CreateOrderCommandHandler(
    IRepository<Order> orderRepository,
    IRepository<Customer> customerRepository,
    IRepository<Product> productRepository,
    IProductResponseCache productResponseCache,
    ICurrentUserService currentUserService,
    IEnumerable<IValidatorService<CreateOrderCommandRequest, CreateOrderCommandResponse>> validators) 
    : RequestHandlerBase<CreateOrderCommandRequest, CreateOrderCommandResponse>(validators)
{
    public override async Task<CreateOrderCommandResponse> Handler(CreateOrderCommandRequest request, CancellationToken cancellationToken)
    {
        // Verify customer exists
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new ArgumentException("مشتری یافت نشد", nameof(request.CustomerId));
        }

        // Generate order number
        var orderNumber = await GenerateOrderNumber(cancellationToken);

        // Create order
        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = request.CustomerId,
            UserId = currentUserService.UserId, // ثبت کاربر ایجاد کننده سفارش
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 0
        };

        var requestedQuantities = request.Items
            .GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));
        var products = new Dictionary<Guid, Product>();

        foreach (var (productId, quantity) in requestedQuantities)
        {
            var product = await productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new ArgumentException($"محصول با شناسه {productId} یافت نشد");
            }

            if (product.Stock < quantity)
            {
                throw new ArgumentException(
                    $"موجودی محصول {product.Name} کافی نیست. موجودی: {product.Stock}، درخواست: {quantity}");
            }

            products.Add(productId, product);
        }

        // Add order items using server-side prices, then decrement each product once.
        decimal totalAmount = 0;
        foreach (var itemDto in request.Items)
        {
            var product = products[itemDto.ProductId];

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);
            totalAmount += orderItem.TotalPrice;
        }

        order.TotalAmount = totalAmount;

        foreach (var (productId, quantity) in requestedQuantities)
        {
            products[productId].Stock -= quantity;
        }

        await orderRepository.AddAsync(entity: order, autoSave: true, cancellationToken: cancellationToken);

        foreach (var productId in requestedQuantities.Keys)
        {
            productResponseCache.Invalidate(productId);
        }

        // Load relations for response
        var savedOrder = await orderRepository.GetQuery()
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        var result = new CreateOrderCommandResponse
        {
            Id = savedOrder!.Id,
            OrderNumber = savedOrder.OrderNumber,
            CustomerId = savedOrder.CustomerId,
            CustomerName = savedOrder.Customer?.Name,
            UserId = savedOrder.UserId,
            UserFullName = savedOrder.User?.FullName,
            OrderDate = savedOrder.OrderDate,
            TotalAmount = savedOrder.TotalAmount,
            Status = savedOrder.Status.ToString(),
            Items = savedOrder.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        return result;
    }

    private async Task<string> GenerateOrderNumber(CancellationToken cancellationToken)
    {
        var orderCount = await orderRepository.GetQuery().IgnoreQueryFilters().CountAsync(cancellationToken);
        var nextNumber = orderCount + 1;

        return $"#{nextNumber:D5}";
    }
}
