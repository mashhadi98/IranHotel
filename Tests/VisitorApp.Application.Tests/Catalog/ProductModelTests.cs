using Microsoft.EntityFrameworkCore;
using VisitorApp.Domain.Features.Catalog.Entities;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Application.Tests.Catalog;

public class ProductModelTests
{
    [Fact]
    public void Product_ShouldOnlyContainNamePriceAndStock()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var product = context.Model.FindEntityType(typeof(Product));

        Assert.NotNull(product);
        Assert.Equal(
            new[] { nameof(Product.Name), nameof(Product.Price), nameof(Product.Stock) },
            product.GetProperties()
                .Where(property => property.Name is not ("Id" or "CreatedAt" or "UpdatedAt" or "RemovedAt" or "Version"))
                .Select(property => property.Name)
                .OrderBy(name => name)
                .ToArray());
        Assert.Null(product.FindProperty("Title"));
        Assert.Null(product.FindProperty("Description"));
        Assert.Null(product.FindProperty("IsActive"));
    }
}
