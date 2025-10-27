using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSecurityIssuesAndCollectionMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkAccesses_Links_LinkId",
                table: "LinkAccesses");

            migrationBuilder.DropIndex(
                name: "IX_LinkAccesses_LinkId",
                table: "LinkAccesses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LinkAccesses_LinkId",
                table: "LinkAccesses",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkAccesses_Links_LinkId",
                table: "LinkAccesses",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
