using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="ISettingService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ISettingService" />
[ApiV1Controller]
[Authorize]
[ExcludeFromCodeCoverage]
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
    [Authorize(Roles = RoleNames.ADMINISTRATION_SETTINGS_VIEW)]
    public async Task<SettingDto> GetSettings(CancellationToken cancellationToken = default)
        => await _modelService.GetSettings(cancellationToken);

    /// <inheritdoc />
    [HttpPut]
    [Authorize(Roles = RoleNames.ADMINISTRATION_SETTINGS_MANAGE)]
    public async Task UpdateSettings(SettingDto settings)
        => await _modelService.UpdateSettings(settings);
}