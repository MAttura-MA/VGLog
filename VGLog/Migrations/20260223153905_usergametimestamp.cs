using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VGLog.Migrations
{
    /// <inheritdoc />
    public partial class usergametimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStampAdded",
                table: "UserGames",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStampAdded",
                table: "UserGames");
        }
    }
}
