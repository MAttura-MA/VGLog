using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VGLog.Migrations
{
    /// <inheritdoc />
    public partial class FixedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Videogames");

            migrationBuilder.AddColumn<int>(
                name: "GameStatus",
                table: "UserGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameStatus",
                table: "UserGames");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Videogames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
