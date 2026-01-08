using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBS.Infrastructure.Migrations
{
    
    public partial class AddProductIdToBlog : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Blog",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHHGNqW2kQg8w0Tz4ftU4UDISIakFsmrVbXwWoJ7vWrl250gu3tGjX+gcX/M5fI7ng==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Blog");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJvcL0xn4F0PwzuWIt19J37lFzlGpZAeN5R/MXSTrMh3qSh9QXJBVJ1Y8B/KkGdtag==");
        }
    }
}
