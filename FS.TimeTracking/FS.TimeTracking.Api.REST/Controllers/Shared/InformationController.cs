using FS.TimeTracking.Abstractions.DTOs.Configuration;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <inheritdoc cref="IInformationApiService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IInformationApiService" />
[ApiV1Controller]
[ExcludeFromCodeCoverage]
public class InformationController : ControllerBase, IInformationApiService
{
    private readonly IInformationApiService _informationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InformationController"/> class.
    /// </summary>
    /// <param name="informationService">The information service.</param>
    public InformationController(IInformationApiService informationService)
        => _informationService = informationService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<ProductInformationDto> GetProductInformation(CancellationToken cancellationToken = default)
        => await _informationService.GetProductInformation(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<string> GetProductName(CancellationToken cancellationToken = default)
        => await _informationService.GetProductName(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<string> GetProductVersion(CancellationToken cancellationToken = default)
        => await _informationService.GetProductVersion(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<string> GetProductCopyright(CancellationToken cancellationToken = default)
        => await _informationService.GetProductCopyright(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<ClientConfigurationDto> GetClientConfiguration(CancellationToken cancellationToken = default)
        => await _informationService.GetClientConfiguration(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [NotFoundWhenEmpty]
    public async Task<JObject> GetTranslations(string language, CancellationToken cancellationToken = default)
        => await _informationService.GetTranslations(language, cancellationToken);
}