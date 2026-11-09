using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeCustomerOrdersQuery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId_CreatedAt",
                table: "Orders",
                columns: new[] { "CustomerId", "RemovedAt", "CreatedAt", "Id" },
                descending: new[] { false, false, true, true })
                .Annotation("SqlServer:Include", new[] { "OrderNumber", "OrderDate", "TotalAmount", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_RemovedAt",
                table: "OrderItems",
                columns: new[] { "OrderId", "RemovedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_RemovedAt",
                table: "OrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId_CreatedAt",
                table: "Orders",
                columns: new[] { "CustomerId", "RemovedAt", "CreatedAt" },
                descending: new[] { false, false, true })
                .Annotation("SqlServer:Include", new[] { "OrderNumber", "OrderDate", "TotalAmount", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }
    }
}
