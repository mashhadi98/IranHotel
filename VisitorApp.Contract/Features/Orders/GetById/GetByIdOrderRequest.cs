using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Orders.GetById;

public class GetByIdOrderRequest() : RequestBase("Orders/{Id}", ApiTypes.Get)
{
    public Guid Id { get; set; }
}
