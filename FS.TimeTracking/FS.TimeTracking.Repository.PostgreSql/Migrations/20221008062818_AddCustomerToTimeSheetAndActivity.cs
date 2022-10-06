using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace FS.TimeTracking.Repository.PostgreSql.Migrations
{
    public partial class AddCustomerToTimeSheetAndActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "TimeSheets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "TimeSheets",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""TimeSheets""
                SET ""CustomerId"" = (SELECT ""CustomerId"" FROM ""Projects"" WHERE ""Id"" = ""TimeSheets"".""ProjectId"");");

            migrationBuilder.AlterColumn<string>(
                "CustomerId",
                "TimeSheets",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Projects",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Activities",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Activities""
                SET ""CustomerId"" = (SELECT ""CustomerId"" FROM ""Projects"" WHERE ""Id"" = ""Activities"".""ProjectId"");");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "TimeSheets",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Projects",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
