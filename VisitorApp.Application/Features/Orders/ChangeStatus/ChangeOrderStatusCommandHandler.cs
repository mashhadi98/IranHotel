using VisitorApp.Contract.Features.Orders.ChangeStatus;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Application.Features.Orders.ChangeStatus;

public class ChangeOrderStatusCommandHandler(IRepository<Order> repository)
    : RequestHandlerBase<ChangeOrderStatusCommandRequest, ChangeOrderStatusResponse>()
{
    public override async Task<ChangeOrderStatusResponse> Handler(ChangeOrderStatusCommandRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: false, out var status) ||
            !Enum.IsDefined(status) ||
            !string.Equals(Enum.GetName(status), request.Status, StringComparison.Ordinal))
        {
            throw new ArgumentException("وضعیت سفارش باید یکی از Pending، Confirmed، Cancelled یا Completed باشد", nameof(request.Status));
        }

        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException("شناسه سفارش الزامی است", nameof(request.Id));
        }

        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
        {
            throw new KeyNotFoundException("سفارش یافت نشد");
        }

        order.Status = status;
        await repository.UpdateAsync(order, autoSave: true, cancellationToken: cancellationToken);

        return new ChangeOrderStatusResponse
        {
            Id = order.Id,
            Status = order.Status.ToString()
        };
    }
}
