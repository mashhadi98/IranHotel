using Microsoft.EntityFrameworkCore;
using VisitorApp.Contract.Features.Orders.Common;
using VisitorApp.Contract.Features.Orders.GetById;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Application.Features.Orders.GetById;

public class GetByIdOrderQueryHandler(IRepository<Order> repository) 
    : RequestHandlerBase<GetByIdOrderQueryRequest, GetByIdOrderQueryResponse>()
{
    public override async Task<GetByIdOrderQueryResponse> Handler(GetByIdOrderQueryRequest request, CancellationToken cancellationToken)
    {
        var order = await repository.GetQuery()
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
        {
            throw new ArgumentException("سفارش یافت نشد", nameof(request.Id));
        }

        var result = new GetByIdOrderQueryResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name,
            CustomerEmail = order.Customer?.Email,
            UserId = order.UserId,
            UserFullName = order.User?.FullName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            Items = order.Items.Select(i => new OrderItemDto
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
}
