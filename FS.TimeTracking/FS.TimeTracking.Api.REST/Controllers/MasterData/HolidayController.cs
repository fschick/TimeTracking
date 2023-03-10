using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IHolidayApiService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IHolidayApiService" />
[ApiV1Controller]
[Authorize(Policy = PermissionName.MASTER_DATA_HOLIDAYS)]
[ExcludeFromCodeCoverage]
public class HolidayController : CrudModelController<HolidayDto, HolidayGridDto>, IHolidayApiService
{
    private readonly IHolidayApiService _holidayService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HolidayController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public HolidayController(IHolidayApiService modelService)
        : base(modelService)
    {
        _holidayService = modelService;
    }

    /// <inheritdoc />
    [HttpPost]
    [Authorize(Roles = RoleName.MASTER_DATA_HOLIDAYS_MANAGE)]
    public async Task Import([Required] IFormFile file, [Required] HolidayType type, CancellationToken cancellationToken = default)
        => await _holidayService.Import(file, type, cancellationToken);
}