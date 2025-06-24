using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalEquipmentProject.Migrations
{
    public partial class AddProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chỉ giữ lại phần tạo bảng Products
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            // Chỉ giữ lại phần tạo bảng ProductImages
            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Giữ lại dữ liệu mẫu
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Date", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 19, 0, 0, 0, 0, DateTimeKind.Local), "Bàn làm việc", 2500000m, 10 },
                    { 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Local), "Ghế văn phòng", 1500000m, 15 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "ImageUrl", "ProductId" },
                values: new object[,]
                {
                    { 1, "/product-images/1.sm.webp", 1 },
                    { 2, "/product-images/2.webp", 2 }
                });

            // Chỉ giữ lại index cần thiết
            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Sửa lại để chỉ xóa 2 bảng này khi rollback
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}