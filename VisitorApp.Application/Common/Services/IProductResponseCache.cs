using VisitorApp.Contract.Features.Products.GetById;

namespace VisitorApp.Application.Common.Services;

public interface IProductResponseCache
{
    Task<ProductResponse?> GetOrCreateAsync(
        Guid productId,
        Func<CancellationToken, Task<ProductResponse?>> factory,
        CancellationToken cancellationToken);

    void Invalidate(Guid productId);
}
