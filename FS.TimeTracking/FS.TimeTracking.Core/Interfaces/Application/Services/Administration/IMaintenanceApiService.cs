using FS.TimeTracking.Core.Models.Filter;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Administration;

/// <summary>
/// Data maintenance services.
/// </summary>
public interface IMaintenanceApiService
{
    /// <summary>
    /// Exports the database.
    /// </summary>
    Task<JObject> ExportData(TimeSheetFilterSet filters);

    /// <summary>
    /// Truncates all data from database and imports new data.
    /// </summary>
    /// <param name="data">The data to import.</param>
    Task ImportData(JObject data);

    /// <summary>
    /// Truncates all data from database.
    /// </summary>
    Task TruncateData();
}