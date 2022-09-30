using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.MySql.Migrations
{
    public partial class DiffSecondsTimeZoneHandling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EDITED
            migrationBuilder.Sql(
                "DROP FUNCTION DiffSeconds");

            // EDITED
            migrationBuilder.Sql(@"
                CREATE FUNCTION DiffSeconds(fromDate datetime(6), fromOffset int, toDate datetime(6), toOffset int)
                    RETURNS bigint(20) unsigned
                    DETERMINISTIC
                    RETURN (SELECT TIMESTAMPDIFF(SECOND , ToUtc(fromDate, fromOffset), COALESCE (ToUtc(toDate, toOffset), UTC_TIMESTAMP())));");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // EDITED
            migrationBuilder.Sql(
                "DROP FUNCTION DiffSeconds");

            // EDITED
            migrationBuilder.Sql(@"
                CREATE FUNCTION DiffSeconds(fromDate datetime(6), offset int, toDate datetime(6))
                    RETURNS bigint unsigned
                    DETERMINISTIC
                    RETURN (SELECT TIMESTAMPDIFF(SECOND , fromDate, COALESCE (toDate, UTC_TIMESTAMP() +  INTERVAL offset  MINUTE)));");
        }
    }
}
