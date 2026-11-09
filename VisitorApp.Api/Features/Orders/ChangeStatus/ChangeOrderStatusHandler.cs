using VisitorApp.Application.Features.Orders.ChangeStatus;
using VisitorApp.Contract.Features.Orders.ChangeStatus;

namespace VisitorApp.API.Features.Orders.ChangeStatus;

public class ChangeOrderStatusHandler : PatchEndpoint<ChangeOrderStatusRequest, ChangeOrderStatusCommandRequest, ChangeOrderStatusResponse>
{
    public override string? RolesAccess => "Admin";

    public ChangeOrderStatusHandler(ISender sender, AutoMapper.IMapper mapper) : base(sender, mapper)
    {
    }
}
