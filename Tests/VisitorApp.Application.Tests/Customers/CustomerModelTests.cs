using Microsoft.EntityFrameworkCore;
using VisitorApp.Domain.Features.Customers.Entities;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Application.Tests.Customers;

public class CustomerModelTests
{
    [Fact]
    public void Customer_ShouldUseUniqueEmailAndName()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost;Database=VisitorAppTests;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        using var context = new ApplicationDbContext(options);
        var customer = context.Model.FindEntityType(typeof(Customer));

        Assert.NotNull(customer);
        Assert.NotNull(customer.FindProperty(nameof(Customer.Name)));
        Assert.NotNull(customer.FindProperty(nameof(Customer.Email)));
        Assert.Null(customer.FindProperty("PhoneNumber"));

        var emailIndex = customer.GetIndexes()
            .Single(index => index.Properties.Any(property => property.Name == nameof(Customer.Email)));
        Assert.True(emailIndex.IsUnique);
    }
}
