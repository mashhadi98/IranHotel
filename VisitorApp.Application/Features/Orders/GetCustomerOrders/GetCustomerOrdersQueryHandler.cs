using VisitorApp.Contract.Features.Orders.GetCustomerOrders;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Application.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersQueryHandler(IRepository<Order> repository)
    : RequestHandlerBase<GetCustomerOrdersQueryRequest, PaginatedResponse<GetCustomerOrdersQueryResponse>>()
{
    public override Task<PaginatedResponse<GetCustomerOrdersQueryResponse>> Handler(GetCustomerOrdersQueryRequest request, CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("شناسه مشتری الزامی است", nameof(request.CustomerId));
        }

        return repository.GetListPaginatedAsync(request, order => order.CustomerId == request.CustomerId, order => new GetCustomerOrdersQueryResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.Name,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            ItemCount = order.Items.Count
        }, cancellationToken);
    }
}
