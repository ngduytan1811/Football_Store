using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductReviewFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJvcL0xn4F0PwzuWIt19J37lFzlGpZAeN5R/MXSTrMh3qSh9QXJBVJ1Y8B/KkGdtag==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEs5DE3W5NyPcuBl0P1+nI0jG0MlaySIlWU3qViJLFpAstwmlNPiPfmZg1wOgQY8FQ==");
        }
    }
}
