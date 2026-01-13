using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBS.Infrastructure.Migrations
{
    public partial class addProductSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Chuẩn hóa dữ liệu Size (null -> "")
            migrationBuilder.UpdateData(
                table: "ProductSizes",
                keyColumn: "Size",
                keyValue: null,
                column: "Size",
                value: "");

            // 2️⃣ Alter Size: NOT NULL + MaxLength
            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "ProductSizes",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // 3️⃣ Alter Color: NOT NULL + MaxLength
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ProductColors",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // 4️⃣ UNIQUE INDEX: ProductColorId + Size
            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_ProductColorId_Size",
                table: "ProductSizes",
                columns: new[] { "ProductColorId", "Size" },
                unique: true);

            // 5️⃣ FK ProductSizes -> ProductColors
            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_ProductColors_ProductColorId",
                table: "ProductSizes",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback FK
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_ProductColors_ProductColorId",
                table: "ProductSizes");

            // Rollback Index
            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_ProductColorId_Size",
                table: "ProductSizes");

            // Rollback Size
            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "ProductSizes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // Rollback Color
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ProductColors",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
