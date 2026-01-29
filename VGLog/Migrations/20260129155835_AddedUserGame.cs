using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VGLog.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videogames_AspNetUsers_UserId",
                table: "Videogames");

            migrationBuilder.DropIndex(
                name: "IX_Videogames_UserId",
                table: "Videogames");

            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "Videogames");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Videogames");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Videogames");

            migrationBuilder.DropColumn(
                name: "PersonalRating",
                table: "Videogames");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Videogames");

            migrationBuilder.CreateTable(
                name: "UserGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    VideogameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonalRating = table.Column<int>(type: "INTEGER", nullable: true),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGames_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGames_Videogames_VideogameId",
                        column: x => x.VideogameId,
                        principalTable: "Videogames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId_VideogameId",
                table: "UserGames",
                columns: new[] { "UserId", "VideogameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_VideogameId",
                table: "UserGames",
                column: "VideogameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGames");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "Videogames",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Videogames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Videogames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonalRating",
                table: "Videogames",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Videogames",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Videogames_UserId",
                table: "Videogames",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videogames_AspNetUsers_UserId",
                table: "Videogames",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
