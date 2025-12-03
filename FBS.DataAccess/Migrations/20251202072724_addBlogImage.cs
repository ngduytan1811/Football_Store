using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBlogImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BlogId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Image = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogImage_Blog_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEs5DE3W5NyPcuBl0P1+nI0jG0MlaySIlWU3qViJLFpAstwmlNPiPfmZg1wOgQY8FQ==");

            migrationBuilder.CreateIndex(
                name: "IX_BlogImage_BlogId",
                table: "BlogImage",
                column: "BlogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogImage");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENVRECwQw8Xd7AseCYe3XxbSz+KrbYzerDDtUuaJxM2qmtx4k0I2S9TYZpLnlAazQQ==");
        }
    }
}
