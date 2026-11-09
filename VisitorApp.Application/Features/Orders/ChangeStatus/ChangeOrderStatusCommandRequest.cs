using VisitorApp.Contract.Features.Orders.ChangeStatus;

namespace VisitorApp.Application.Features.Orders.ChangeStatus;

public class ChangeOrderStatusCommandRequest : IRequestBase<ChangeOrderStatusResponse>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
