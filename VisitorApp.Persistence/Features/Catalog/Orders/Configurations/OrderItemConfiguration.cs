using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisitorApp.Domain.Features.Orders.Entities;

namespace VisitorApp.Persistence.Features.Catalog.Orders.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasQueryFilter(d => d.RemovedAt == null);

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(oi => oi.TotalPrice);

        builder.HasIndex(oi => new { oi.OrderId, oi.RemovedAt })
            .HasDatabaseName("IX_OrderItems_OrderId_RemovedAt");

        // Relationship with Order
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Product
        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}