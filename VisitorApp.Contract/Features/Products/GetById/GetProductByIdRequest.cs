using VisitorApp.Contract.Common;

namespace VisitorApp.Contract.Features.Products.GetById;

public sealed class GetProductByIdRequest() : RequestBase("Products/{Id}", ApiTypes.Get)
{
    public Guid Id { get; set; }
}
