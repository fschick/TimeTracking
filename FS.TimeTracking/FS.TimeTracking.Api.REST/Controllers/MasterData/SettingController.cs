using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="ISettingService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ISettingService" />
[V1ApiController]
public class SettingController : ISettingService
{
    private readonly ISettingService _modelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public SettingController(ISettingService modelService)
        => _modelService = modelService;

    /// <inheritdoc />
    [HttpGet]
    [NotFoundWhenEmpty]
    public async Task<SettingDto> GetSettings(CancellationToken cancellationToken = default)
        => await _modelService.GetSettings(cancellationToken);

    /// <inheritdoc />
    [HttpPut]
    public async Task UpdateSettings(SettingDto settings)
        => await _modelService.UpdateSettings(settings);

    /// <inheritdoc />
    [HttpGet]
    [NotFoundWhenEmpty]
    public async Task<JObject> GetTranslations(string language, CancellationToken cancellationToken = default)
        => await _modelService.GetTranslations(language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<FeatureConfiguration> GetFeatures(CancellationToken cancellationToken = default)
        => await _modelService.GetFeatures(cancellationToken);
}