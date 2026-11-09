using Microsoft.Extensions.Caching.Memory;
using VisitorApp.Application.Common.Services;
using VisitorApp.Contract.Features.Products.GetById;

namespace VisitorApp.API.Common.Services;

public sealed class ProductResponseCache(IMemoryCache memoryCache) : IProductResponseCache
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public async Task<ProductResponse?> GetOrCreateAsync(
        Guid productId,
        Func<CancellationToken, Task<ProductResponse?>> factory,
        CancellationToken cancellationToken)
    {
        var key = GetKey(productId);
        if (memoryCache.TryGetValue(key, out ProductResponse? cached))
        {
            return cached;
        }

        var product = await factory(cancellationToken);
        if (product is not null)
        {
            memoryCache.Set(key, product, CacheDuration);
        }

        return product;
    }

    public void Invalidate(Guid productId) => memoryCache.Remove(GetKey(productId));

    private static string GetKey(Guid productId) => $"product-response:{productId:N}";
}
