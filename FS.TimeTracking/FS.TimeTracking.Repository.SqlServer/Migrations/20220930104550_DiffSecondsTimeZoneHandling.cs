using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.TimeTracking.Repository.SqlServer.Migrations
{
    public partial class DiffSecondsTimeZoneHandling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EDITED
            migrationBuilder.Sql(
                "DROP FUNCTION dbo.DiffSeconds");

            // EDITED
            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.DiffSeconds (
	                @fromDate DATETIME2,
	                @fromOffset INT,
	                @toDate DATETIME2,
                    @toOffset INT
                )
                RETURNS DECIMAL(20,0) AS
                BEGIN
	                DECLARE @fromDateUtc DATETIME2 = dbo.ToUtc(@fromDate, @fromOffset)
		            DECLARE @toDateUtc DATETIME2 = dbo.ToUtc(@toDate, @toOffset)
	                RETURN CAST(DATEDIFF_BIG(SECOND, @fromDateUtc, COALESCE(@toDateUtc, GETUTCDATE())) AS DECIMAL(20,0))
                END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // EDITED
            migrationBuilder.Sql(
                "DROP FUNCTION dbo.DiffSeconds");

            // EDITED
            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.DiffSeconds (
	                @fromDate DATETIME2,
	                @offset INT,
	                @toDate DATETIME2
                )
                RETURNS DECIMAL(20,0) AS
                BEGIN
	                RETURN CAST(DATEDIFF_BIG(SECOND, @fromDate, COALESCE(@toDate, DATEADD(MINUTE, @offset, GETUTCDATE()))) AS DECIMAL(20,0))
                END;");
        }
    }
}
