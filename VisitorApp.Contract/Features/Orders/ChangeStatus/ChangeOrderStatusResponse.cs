namespace VisitorApp.Contract.Features.Orders.ChangeStatus;

public class ChangeOrderStatusResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
