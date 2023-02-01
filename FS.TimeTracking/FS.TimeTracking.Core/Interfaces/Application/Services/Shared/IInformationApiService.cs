using FS.TimeTracking.Abstractions.DTOs.Configuration;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Information services.
/// </summary>
public interface IInformationApiService
{
    /// <summary>
    /// Gets the name, version and copyright of the product.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<ProductInformationDto> GetProductInformation(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<string> GetProductName(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the product version.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<string> GetProductVersion(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the copyright for the product.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<string> GetProductCopyright(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets configuration values for client.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ClientConfigurationDto> GetClientConfiguration(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the translation for a specific language.
    /// </summary>
    /// <param name="language">The language to get the translations for.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<JObject> GetTranslations(string language, CancellationToken cancellationToken = default);
}