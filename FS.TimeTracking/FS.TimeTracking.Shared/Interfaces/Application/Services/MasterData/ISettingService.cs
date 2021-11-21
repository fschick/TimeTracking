using FS.TimeTracking.Shared.DTOs.TimeTracking;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.DTOs.MasterData;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;

/// <summary>
/// Service to read and persists settings
/// </summary>
public interface ISettingService
{
    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<SettingDto> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the settings.
    /// </summary>
    Task Update(SettingDto settings);
}