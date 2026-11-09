using Microsoft.EntityFrameworkCore;
using VisitorApp.Contract.Features.Orders.GetCustomerOrders;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Application.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersQueryHandler(IRepository<Order> repository)
    : RequestHandlerBase<GetCustomerOrdersQueryRequest, CursorPaginatedResponse<GetCustomerOrdersQueryResponse>>()
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public override async Task<CursorPaginatedResponse<GetCustomerOrdersQueryResponse>> Handler(
        GetCustomerOrdersQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("شناسه مشتری الزامی است", nameof(request.CustomerId));
        }

        if (request.AfterCreatedAt.HasValue != request.AfterId.HasValue)
        {
            throw new ArgumentException("هر دو مقدار AfterCreatedAt و AfterId باید ارسال شوند", nameof(request));
        }

        if (request.Page is <= 0 or > 1)
        {
            throw new ArgumentException("صفحه‌بندی offset حذف شده است؛ از cursor دریافتی استفاده کنید", nameof(request.Page));
        }

        if (request.PageSize is <= 0)
        {
            throw new ArgumentException("PageSize باید بزرگتر از صفر باشد", nameof(request.PageSize));
        }

        var pageSize = Math.Min(request.PageSize ?? DefaultPageSize, MaxPageSize);
        var query = repository.GetQuery()
            .AsNoTracking()
            .Where(order => order.CustomerId == request.CustomerId);

        if (request.AfterCreatedAt.HasValue && request.AfterId.HasValue)
        {
            var afterCreatedAt = request.AfterCreatedAt.Value;
            var afterId = request.AfterId.Value;
            query = query.Where(order =>
                order.CreatedAt < afterCreatedAt ||
                (order.CreatedAt == afterCreatedAt && order.Id.CompareTo(afterId) < 0));
        }

        var orders = await query
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .Select(order => new GetCustomerOrdersQueryResponse
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.Name,
                CreatedAt = order.CreatedAt,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                ItemCount = order.Items.Count
            })
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var hasMore = orders.Count > pageSize;
        if (hasMore)
        {
            orders.RemoveAt(pageSize);
        }

        Cursor? nextCursor = hasMore && orders.Count > 0
            ? new Cursor(orders[^1].CreatedAt, orders[^1].Id)
            : null;

        return new CursorPaginatedResponse<GetCustomerOrdersQueryResponse>(orders, hasMore, nextCursor);
    }
}
