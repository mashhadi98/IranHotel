using Microsoft.Extensions.Caching.Memory;
using VisitorApp.API.Common.Services;
using VisitorApp.Contract.Features.Products.GetById;

namespace VisitorApp.Application.Tests.Api;

public class ProductResponseCacheTests
{
    [Fact]
    public async Task GetOrCreateAsync_CachesUntilInvalidated()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new ProductResponseCache(memoryCache);
        var productId = Guid.NewGuid();
        var factoryCalls = 0;

        Task<ProductResponse?> Load(CancellationToken _) =>
            Task.FromResult<ProductResponse?>(new ProductResponse
            {
                Id = productId,
                Name = "Product",
                Price = 10m,
                Stock = ++factoryCalls
            });

        var first = await cache.GetOrCreateAsync(productId, Load, CancellationToken.None);
        var cached = await cache.GetOrCreateAsync(productId, Load, CancellationToken.None);

        Assert.Equal(1, factoryCalls);
        Assert.Equal(first!.Stock, cached!.Stock);

        cache.Invalidate(productId);
        var refreshed = await cache.GetOrCreateAsync(productId, Load, CancellationToken.None);

        Assert.Equal(2, factoryCalls);
        Assert.Equal(2, refreshed!.Stock);
    }
}
