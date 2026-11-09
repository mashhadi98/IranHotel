using VisitorApp.Application.Features.Orders.Create;
using VisitorApp.Contract.Features.Orders.Create;

namespace VisitorApp.API.Features.Orders.Create;

public class CreateOrderHandler : PostEndpoint<CreateOrderRequest, CreateOrderCommandRequest, CreateOrderCommandResponse>
{
    public override string? RolesAccess => "Admin";
    
    public CreateOrderHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
}
