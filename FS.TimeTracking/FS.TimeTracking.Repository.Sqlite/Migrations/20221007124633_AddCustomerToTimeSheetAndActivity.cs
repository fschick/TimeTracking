using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.Sqlite.Migrations
{
    public partial class AddCustomerToTimeSheetAndActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProjectId",
                table: "TimeSheets",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "TimeSheets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE TimeSheets
                SET CustomerId = (SELECT CustomerId FROM Projects WHERE Id = TimeSheets.ProjectId);");

            migrationBuilder.AlterColumn<string>(
                "CustomerId",
                "TimeSheets",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Projects",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Activities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Activities
                SET CustomerId = (SELECT CustomerId FROM Projects WHERE Id = Activities.ProjectId);");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheets_CustomerId",
                table: "TimeSheets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CustomerId",
                table: "Activities",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Customers_CustomerId",
                table: "Activities",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheets_Customers_CustomerId",
                table: "TimeSheets",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Customers_CustomerId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheets_Customers_CustomerId",
                table: "TimeSheets");

            migrationBuilder.DropIndex(
                name: "IX_TimeSheets_CustomerId",
                table: "TimeSheets");

            migrationBuilder.DropIndex(
                name: "IX_Activities_CustomerId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "TimeSheets");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectId",
                table: "TimeSheets",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
