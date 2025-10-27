using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRolesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "User");

            migrationBuilder.Sql(@"
                UPDATE Users 
                SET Roles = 'User' 
                WHERE Roles IS NULL OR Roles = ''
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Users");
        }
    }
}
