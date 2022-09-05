using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.PostgreSql.Migrations
{
    public partial class CompanyAddedToSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Settings\" SET \"Key\" = 'Company.Currency' WHERE \"Key\" = 'Currency';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Monday', CASE WHEN POSITION('\"Monday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Tuesday', CASE WHEN POSITION('\"Tuesday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Wednesday', CASE WHEN POSITION('\"Wednesday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Thursday', CASE WHEN POSITION('\"Thursday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Friday', CASE WHEN POSITION('\"Friday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Saturday', CASE WHEN POSITION('\"Saturday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("INSERT INTO \"Settings\" (\"Key\", \"Value\", \"Created\", \"Modified\") SELECT 'Workdays.Sunday', CASE WHEN POSITION('\"Sunday\"' IN \"Value\") > 0 THEN 'true' ELSE 'false' END, \"Created\", \"Modified\" FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Settings\" SET \"Key\" = 'Currency' WHERE \"Key\" = 'Company.Currency';");
            migrationBuilder.Sql("UPDATE \"Settings\" SET \"Key\" = 'Workdays',\t\"Value\" = (SELECT CONCAT('{\"Monday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Monday'), ',\"Tuesday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Tuesday'), ',\"Wednesday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Wednesday'), ',\"Thursday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Thursday'), ',\"Friday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Friday'), ',\"Saturday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Saturday'), ',\"Sunday\":', (SELECT \"Value\" FROM \"Settings\" WHERE \"Key\" = 'Workdays.Sunday'), '}') FROM \"Settings\" s WHERE \"Key\" = 'Workdays.Monday') WHERE \"Key\" = 'Workdays.Monday';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays.Tuesday';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays.Wednesday';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays.Thursday';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays.Friday';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays.Saturday';");
            migrationBuilder.Sql("DELETE FROM \"Settings\" WHERE \"Key\" = 'Workdays.Sunday';");
        }
    }
}