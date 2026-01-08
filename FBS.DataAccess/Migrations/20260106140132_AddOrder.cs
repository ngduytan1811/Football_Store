using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEM30zLtXS+52QcLcbbqD/Eq/Hh1noB5Y2yl+s1VCklUPIOMkqk/5bDYhXvP65lMoAw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPAEfgys880XR5pWT34hzWuyxkzN2YnvKIq3p9g+QtP5S19v/HPHJSLAei/KD7FPtw==");
        }
    }
}
