using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.Sqlite.Migrations
{
    public partial class UserAddedToTimeSheetAndHoliday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TimeSheets",
                type: "TEXT",
                nullable: false,
                defaultValue: "00000000-0000-0000-0000-000000000000");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Holidays",
                type: "TEXT",
                nullable: false,
                defaultValue: "00000000-0000-0000-0000-000000000000");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TimeSheets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Holidays");
        }
    }
}
