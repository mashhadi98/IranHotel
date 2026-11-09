using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Encodings.Web;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VisitorApp.API.Common.Configuration;
using VisitorApp.API.Common.Middlewares;
using VisitorApp.Domain.Features.Catalog.Entities;
using VisitorApp.Domain.Features.Customers.Entities;
using VisitorApp.Persistence.Common.Context;

namespace VisitorApp.Application.Tests.Api;

public class OrderApiTests
{
    [Fact]
    public async Task OrderEndpoints_CreateReadAndChangeStatus()
    {
        await using var host = await OrderApiHost.CreateAsync();
        using var client = host.Client;

        var createResponse = await client.PostAsJsonAsync("/api/Orders", new
        {
            customerId = host.CustomerId,
            items = new[] { new { productId = host.ProductId, quantity = 2 } }
        }, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        using var createJson = await JsonDocument.ParseAsync(
            await createResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
            cancellationToken: TestContext.Current.CancellationToken);
        var createdOrder = createJson.RootElement.GetProperty("data");
        var orderId = createdOrder.GetProperty("id").GetGuid();
        Assert.Equal(30m, createdOrder.GetProperty("totalAmount").GetDecimal());

        var secondCreateResponse = await client.PostAsJsonAsync("/api/Orders", new
        {
            customerId = host.CustomerId,
            items = new[] { new { productId = host.ProductId, quantity = 1 } }
        }, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, secondCreateResponse.StatusCode);

        var getOrderResponse = await client.GetAsync(
            $"/api/Orders/{orderId}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, getOrderResponse.StatusCode);

        var customerOrdersResponse = await client.GetAsync(
            $"/api/Customers/{host.CustomerId}/Orders?pageSize=1",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, customerOrdersResponse.StatusCode);
        using var customerOrdersJson = await JsonDocument.ParseAsync(
            await customerOrdersResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Single(customerOrdersJson.RootElement.GetProperty("data").GetProperty("list").EnumerateArray());
        var firstPage = customerOrdersJson.RootElement.GetProperty("data");
        Assert.True(firstPage.GetProperty("hasMore").GetBoolean());
        var cursor = firstPage.GetProperty("nextCursor");
        var cursorTimestamp = Uri.EscapeDataString(cursor.GetProperty("createdAt").GetString()!);
        var cursorId = cursor.GetProperty("id").GetGuid();

        var nextPageResponse = await client.GetAsync(
            $"/api/Customers/{host.CustomerId}/Orders?pageSize=1&afterCreatedAt={cursorTimestamp}&afterId={cursorId}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, nextPageResponse.StatusCode);
        using var nextPageJson = await JsonDocument.ParseAsync(
            await nextPageResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Single(nextPageJson.RootElement.GetProperty("data").GetProperty("list").EnumerateArray());

        var changeStatusResponse = await client.PatchAsJsonAsync(
            $"/api/Orders/{orderId}/Status",
            new { status = "Confirmed" },
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, changeStatusResponse.StatusCode);
    }

    [Fact]
    public async Task CreateOrderEndpoint_InvalidRequest_ReturnsBadRequest()
    {
        await using var host = await OrderApiHost.CreateAsync();
        using var response = await host.Client.PostAsJsonAsync("/api/Orders", new
        {
            customerId = host.CustomerId,
            items = new[] { new { productId = host.ProductId, quantity = 0 } }
        }, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed class OrderApiHost : IAsyncDisposable
    {
        private readonly WebApplication app;

        private OrderApiHost(WebApplication app, Guid customerId, Guid productId)
        {
            this.app = app;
            CustomerId = customerId;
            ProductId = productId;
            Client = app.GetTestClient();
        }

        public HttpClient Client { get; }
        public Guid CustomerId { get; }
        public Guid ProductId { get; }

        public static async Task<OrderApiHost> CreateAsync()
        {
            var databaseName = $"orders-api-{Guid.NewGuid():N}";
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            builder.Services.AddFastEndpoints(options =>
                options.Assemblies = [typeof(VisitorApp.API.Features.Orders.Create.CreateOrderHandler).Assembly]);
            builder.Services.AddApplicationServices();
            builder.Services.AddMappingServices();
            builder.Services.AddDependencyInjectionServices();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));
            builder.Services.AddIdentityServices();
            builder.Services.AddOpenIddictServices(builder.Configuration);
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = TestAuthenticationHandler.TestScheme;
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.TestScheme;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.TestScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.TestScheme, _ => { });
            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseStandardResponseWrapper();
            app.UseGlobalErrorHandling();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseFastEndpoints(options => options.Endpoints.RoutePrefix = "api");

            Guid customerId;
            Guid productId;
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
                var customer = new Customer { Name = "API Customer", Email = $"{Guid.NewGuid():N}@example.test" };
                var product = new Product { Name = "API Product", Price = 15m, Stock = 10 };
                context.Customers.Add(customer);
                context.Products.Add(product);
                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
                customerId = customer.Id;
                productId = product.Id;
            }

            await app.StartAsync(TestContext.Current.CancellationToken);
            return new OrderApiHost(app, customerId, productId);
        }

        public async ValueTask DisposeAsync()
        {
            Client.Dispose();
            await app.DisposeAsync();
        }
    }

    private sealed class TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string TestScheme = "OrderApiTest";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, "api-test"), new Claim(ClaimTypes.Role, "Admin")],
                TestScheme);
            var principal = new ClaimsPrincipal(identity);
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(principal, TestScheme)));
        }
    }
}
