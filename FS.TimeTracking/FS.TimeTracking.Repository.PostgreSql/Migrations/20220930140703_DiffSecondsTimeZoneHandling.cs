using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.PostgreSql.Migrations
{
    public partial class DiffSecondsTimeZoneHandling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EDITED
            migrationBuilder.Sql(
                "DROP FUNCTION diffseconds(timestamp, int, timestamp)");

            // EDITED
            migrationBuilder.Sql(@"
                CREATE FUNCTION public.diffseconds(from_date timestamp without time zone, from_offset integer, to_date timestamp without time zone, to_offset integer)
                RETURNS numeric
                LANGUAGE plpgsql
                AS
                $$
                DECLARE
	                from_date_utc timestamp;
	                to_date_utc timestamp;
                BEGIN
	                from_date_utc = toUtc(from_date, from_offset);
	                to_date_utc = toUtc(to_date, to_offset);
	                RETURN CAST(EXTRACT(EPOCH FROM (COALESCE(to_date_utc, NOW() AT TIME ZONE 'utc') - from_date_utc)) AS NUMERIC(20,0));
                END;
                $$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // EDITED
            migrationBuilder.Sql(
                "DROP FUNCTION diffseconds(timestamp, int, timestamp)");

            // EDITED
            migrationBuilder.Sql(@"
                CREATE function diffseconds(timestamp, int, timestamp) RETURNS numeric(20,0) AS
                    $$ SELECT CAST(EXTRACT(EPOCH FROM (COALESCE($3, NOW() AT TIME ZONE 'utc' + ($2 * INTERVAL '1 minute')) - $1)) AS NUMERIC(20,0)); $$
                    LANGUAGE sql
                    IMMUTABLE;");
        }
    }
}
