using Microsoft.EntityFrameworkCore;
using VisitorApp.Application.Common.Services;
using VisitorApp.Contract.Features.Products.GetById;
using VisitorApp.Domain.Features.Catalog.Entities;

namespace VisitorApp.Application.Features.Products.GetById;

public sealed class GetProductByIdQueryHandler(
    IRepository<Product> repository,
    IProductResponseCache cache)
    : RequestHandlerBase<GetProductByIdQueryRequest, ProductResponse>()
{
    public override async Task<ProductResponse> Handler(
        GetProductByIdQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException("شناسه محصول معتبر نیست", nameof(request.Id));
        }

        var product = await cache.GetOrCreateAsync(
            request.Id,
            async token => await repository.GetQuery()
                .AsNoTracking()
                .Where(item => item.Id == request.Id)
                .Select(item => new ProductResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Stock = item.Stock
                })
                .FirstOrDefaultAsync(token),
            cancellationToken);

        return product ?? throw new KeyNotFoundException("محصول یافت نشد");
    }
}
