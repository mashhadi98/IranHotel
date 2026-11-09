using VisitorApp.Domain.Common.Entities;

namespace VisitorApp.Domain.Features.Catalog.Entities;

public class Product : Entity // Guid-based by default
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
