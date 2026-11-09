using VisitorApp.Application.Features.Orders.GetById;
using VisitorApp.Contract.Features.Orders.GetById;

namespace VisitorApp.API.Features.Orders.GetById;

public class GetByIdOrderHandler : GetEndpoint<GetByIdOrderRequest, GetByIdOrderQueryRequest, GetByIdOrderQueryResponse>
{
    public override string? RolesAccess => "Admin";
    
    public GetByIdOrderHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
}
