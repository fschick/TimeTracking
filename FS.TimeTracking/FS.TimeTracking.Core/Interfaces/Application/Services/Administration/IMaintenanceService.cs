using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Administration;

/// <summary>
/// Data maintenance services.
/// </summary>
public interface IMaintenanceService : IMaintenanceApiService
{
    /// <summary>
    /// Resets the database.
    /// </summary>
    Task ResetDatabase();
}