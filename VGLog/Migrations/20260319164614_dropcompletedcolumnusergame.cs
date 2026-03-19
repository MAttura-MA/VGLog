using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VGLog.Migrations
{
    /// <inheritdoc />
    public partial class dropcompletedcolumnusergame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "UserGames");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "UserGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
