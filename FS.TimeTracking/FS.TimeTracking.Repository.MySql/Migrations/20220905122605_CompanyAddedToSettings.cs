using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.MySql.Migrations
{
    public partial class CompanyAddedToSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Settings SET `Key` = 'Company.Currency' WHERE `Key` = 'Currency';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Monday', IF(LOCATE('\"Monday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Tuesday', IF(LOCATE('\"Tuesday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Wednesday', IF(LOCATE('\"Wednesday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Thursday', IF(LOCATE('\"Thursday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Friday', IF(LOCATE('\"Friday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Saturday', IF(LOCATE('\"Saturday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO Settings (`Key`, Value, Created, Modified) SELECT 'Workdays.Sunday', IF(LOCATE('\"Sunday\":true', Value) > 0, 'true', 'false'), Created, Modified FROM Settings WHERE `Key` = 'Workdays';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Settings SET `Key` = 'Currency' WHERE `Key` = 'Company.Currency';");
            migrationBuilder.Sql("UPDATE Settings SET `Key` = 'Workdays',\tValue = (SELECT CONCAT('{\"Monday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Monday'), ',\"Tuesday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Tuesday'), ',\"Wednesday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Wednesday'), ',\"Thursday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Thursday'), ',\"Friday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Friday'), ',\"Saturday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Saturday'), ',\"Sunday\":', (SELECT Value FROM Settings WHERE `Key` = 'Workdays.Sunday'), '}') FROM Settings s WHERE `Key` = 'Workdays.Monday') WHERE `Key` = 'Workdays.Monday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays.Tuesday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays.Wednesday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays.Thursday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays.Friday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays.Saturday';");
            migrationBuilder.Sql("DELETE FROM Settings WHERE `Key` = 'Workdays.Sunday';");
        }
    }
}