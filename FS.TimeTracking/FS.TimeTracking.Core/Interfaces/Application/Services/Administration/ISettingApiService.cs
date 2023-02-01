using FS.TimeTracking.Abstractions.DTOs.MasterData;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Administration;

/// <summary>
/// Service to read and persists settings
/// </summary>
public interface ISettingApiService
{
    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<SettingDto> GetSettings(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the settings.
    /// </summary>
    Task UpdateSettings(SettingDto settings);
}