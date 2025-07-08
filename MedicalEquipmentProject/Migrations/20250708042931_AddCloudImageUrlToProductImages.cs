using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalEquipmentProject.Migrations
{
    /// <inheritdoc />
    public partial class AddCloudImageUrlToProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "CloudImageUrl",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloudImageUrl",
                table: "ProductImages");

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
        }
    }
}
