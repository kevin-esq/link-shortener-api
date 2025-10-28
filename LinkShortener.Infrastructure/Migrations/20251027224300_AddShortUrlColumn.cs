using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShortUrlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortUrl",
                table: "Links",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortUrl",
                table: "Links");
        }
    }
}
