using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Orders.ChangeStatus;

public class ChangeOrderStatusRequest() : RequestBase("Orders/{Id}/Status", ApiTypes.Patch)
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
