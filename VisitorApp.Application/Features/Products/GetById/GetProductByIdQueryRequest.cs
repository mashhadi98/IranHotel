using VisitorApp.Contract.Features.Products.GetById;

namespace VisitorApp.Application.Features.Products.GetById;

public sealed class GetProductByIdQueryRequest : IRequestBase<ProductResponse>
{
    public Guid Id { get; set; }
}
