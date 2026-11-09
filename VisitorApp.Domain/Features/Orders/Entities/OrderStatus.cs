namespace VisitorApp.Domain.Features.Orders.Entities;

public enum OrderStatus
{
    Pending = 0,   // در انتظار
    Confirmed = 1, // تایید شده
    Cancelled = 2,
    Completed = 3
}