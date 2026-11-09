using VisitorApp.Application.Features.Orders.GetCustomerOrders;
using VisitorApp.Contract.Features.Orders.GetCustomerOrders;

namespace VisitorApp.API.Features.Orders.GetCustomerOrders;

public class GetCustomerOrdersHandler : GetEndpoint<GetCustomerOrdersRequest, GetCustomerOrdersQueryRequest, PaginatedResponse<GetCustomerOrdersQueryResponse>>
{
    public override string? RolesAccess => "Admin";

    public GetCustomerOrdersHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
}
