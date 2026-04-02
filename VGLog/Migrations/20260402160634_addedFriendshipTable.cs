using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VGLog.Migrations
{
    /// <inheritdoc />
    public partial class addedFriendshipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserRequesterId = table.Column<string>(type: "TEXT", nullable: false),
                    UserReceiverId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendship_AspNetUsers_UserReceiverId",
                        column: x => x.UserReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendship_AspNetUsers_UserRequesterId",
                        column: x => x.UserRequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserReceiverId",
                table: "Friendship",
                column: "UserReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserRequesterId_UserReceiverId",
                table: "Friendship",
                columns: new[] { "UserRequesterId", "UserReceiverId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendship");
        }
    }
}
