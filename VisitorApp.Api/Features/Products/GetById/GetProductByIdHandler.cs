using VisitorApp.Application.Features.Products.GetById;
using VisitorApp.Contract.Features.Products.GetById;

namespace VisitorApp.API.Features.Products.GetById;

public sealed class GetProductByIdHandler
    : GetEndpoint<GetProductByIdRequest, GetProductByIdQueryRequest, ProductResponse>
{
    public override string? RolesAccess => "Admin";

    public GetProductByIdHandler(ISender sender, AutoMapper.IMapper mapper)
        : base(sender, mapper)
    {
    }
}
