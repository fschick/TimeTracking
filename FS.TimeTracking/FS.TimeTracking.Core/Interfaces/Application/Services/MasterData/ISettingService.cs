using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;

/// <summary>
/// Service to read and persists settings
/// </summary>
public interface ISettingService
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

    /// <summary>
    /// Gets the translation for a specific language.
    /// </summary>
    /// <param name="language">The language to get the translations for.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<JObject> GetTranslations(string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets enabled features.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<FeatureConfiguration> GetFeatures(CancellationToken cancellationToken = default);
}