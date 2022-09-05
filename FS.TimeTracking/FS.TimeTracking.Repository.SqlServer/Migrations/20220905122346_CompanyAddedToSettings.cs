using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.SqlServer.Migrations
{
    public partial class CompanyAddedToSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Settings SET [Key] = 'Company.Currency' WHERE [Key] = 'Currency';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Monday', IIF(CHARINDEX('\"Monday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Tuesday', IIF(CHARINDEX('\"Tuesday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Wednesday', IIF(CHARINDEX('\"Wednesday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Thursday', IIF(CHARINDEX('\"Thursday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Friday', IIF(CHARINDEX('\"Friday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Saturday', IIF(CHARINDEX('\"Saturday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings ([Key], Value, Created, Modified) SELECT 'Workdays.Sunday', IIF(CHARINDEX('\"Sunday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE [Key] = 'Workdays';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Settings SET [Key] = 'Currency' WHERE [Key] = 'Company.Currency';");
            migrationBuilder.Sql("UPDATE Settings SET [Key] = 'Workdays',\tValue = (SELECT CONCAT('{\"Monday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Monday'), ',\"Tuesday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Tuesday'), ',\"Wednesday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Wednesday'), ',\"Thursday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Thursday'), ',\"Friday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Friday'), ',\"Saturday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Saturday'), ',\"Sunday\":', (SELECT Value FROM Settings WHERE [Key] = 'Workdays.Sunday'), '}') FROM Settings s WHERE [Key] = 'Workdays.Monday') WHERE [Key] = 'Workdays.Monday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays.Tuesday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays.Wednesday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays.Thursday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays.Friday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays.Saturday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE [Key] = 'Workdays.Sunday';");
        }
    }
}