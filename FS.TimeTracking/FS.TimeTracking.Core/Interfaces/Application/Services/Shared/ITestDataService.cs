using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Test data services.
/// </summary>
public interface ITestDataService
{
    /// <summary>
    /// Seeds test data to the database.
    /// </summary>
    /// <param name="amount">The amount of data to seed.</param>
    /// <param name="timeZoneId">The time zone used for generated date/time values</param>
    /// <param name="truncateBeforeSeed">if set to <c>true</c> database if truncated before data are seed.</param>
    Task SeedTestData(int amount = 10, string timeZoneId = null, bool truncateBeforeSeed = false);

    /// <summary>
    /// Truncates all data from database.
    /// </summary>
    Task TruncateData();
}