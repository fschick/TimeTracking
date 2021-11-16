using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers;

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
    [NotFoundWhenEmpty]
    [HttpGet]
    public Task<SettingDto> Get(CancellationToken cancellationToken = default)
        => _modelService.Get(cancellationToken);

    /// <inheritdoc />
    [HttpPut]
    public Task Update(SettingDto settings)
        => _modelService.Update(settings);
}