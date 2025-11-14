using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductColors_ProductColorId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductSizes_ProductSizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductColorId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ProductSizeId",
                table: "OrderItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductSizeId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductId");

            migrationBuilder.AddColumn<string>(
                name: "ProductColor",
                table: "OrderItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProductSize",
                table: "OrderItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ1857Ec7qxiIU185ZxpCLmCoVEJTWz28GR/nvD0TbKtOx2MWO2tADbjEMUvxkbCWw==");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductColor",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductSize",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderItems",
                newName: "ProductSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductSizeId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductColorId",
                table: "OrderItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAQo+vfnNuXyRZag9dgYuLxyZKDzT87e6WUPf+tBq+85Bq0aYWKnhIM4uhCaFC8BiQ==");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductColorId",
                table: "OrderItems",
                column: "ProductColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductColors_ProductColorId",
                table: "OrderItems",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductSizes_ProductSizeId",
                table: "OrderItems",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id");
        }
    }
}
