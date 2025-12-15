using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VGLog.Migrations
{
    /// <inheritdoc />
    public partial class FixedPlatformTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.RenameTable(
                name: "platforms",
                newName: "Platforms_tmp");

            migrationBuilder.RenameTable(
                name: "platforms_tmp",
                newName: "Platforms");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameTable(
                name: "Platforms",
                newName: "platforms_tmp");

            migrationBuilder.RenameTable(
                name: "platforms_tmp",
                newName: "platforms");

        }
    }
}
