using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeUserRolesToSecondNormalForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_GrantedBy",
                        column: x => x.GrantedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_GrantedBy",
                table: "UserRoles",
                column: "GrantedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_Role",
                table: "UserRoles",
                columns: new[] { "UserId", "Role" },
                unique: true);

            // Migrate existing roles from CSV to UserRoles table
            migrationBuilder.Sql(@"
                -- Split CSV roles and insert into UserRoles table
                DECLARE @UserId UNIQUEIDENTIFIER
                DECLARE @Roles NVARCHAR(MAX)
                DECLARE @Role NVARCHAR(20)
                DECLARE @Pos INT
                
                DECLARE user_cursor CURSOR FOR 
                SELECT Id, Roles FROM Users WHERE Roles IS NOT NULL AND Roles != ''
                
                OPEN user_cursor
                FETCH NEXT FROM user_cursor INTO @UserId, @Roles
                
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    -- Split CSV and insert each role
                    WHILE LEN(@Roles) > 0
                    BEGIN
                        SET @Pos = CHARINDEX(',', @Roles)
                        
                        IF @Pos > 0
                        BEGIN
                            SET @Role = LTRIM(RTRIM(SUBSTRING(@Roles, 1, @Pos - 1)))
                            SET @Roles = SUBSTRING(@Roles, @Pos + 1, LEN(@Roles))
                        END
                        ELSE
                        BEGIN
                            SET @Role = LTRIM(RTRIM(@Roles))
                            SET @Roles = ''
                        END
                        
                        -- Insert role if not empty
                        IF LEN(@Role) > 0
                        BEGIN
                            INSERT INTO UserRoles (Id, UserId, Role, GrantedAt, GrantedBy)
                            VALUES (NEWID(), @UserId, @Role, GETUTCDATE(), NULL)
                        END
                    END
                    
                    FETCH NEXT FROM user_cursor INTO @UserId, @Roles
                END
                
                CLOSE user_cursor
                DEALLOCATE user_cursor
            ");

            // Drop the old Roles column
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
