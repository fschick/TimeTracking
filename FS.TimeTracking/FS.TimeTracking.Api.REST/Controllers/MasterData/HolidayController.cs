using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Enums;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.DTOs.MasterData;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IHolidayService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IHolidayService" />
[V1ApiController]
public class HolidayController : CrudModelController<HolidayDto, HolidayListDto>, IHolidayService
{
    private readonly IHolidayService _holidayService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HolidayController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public HolidayController(IHolidayService modelService)
        : base(modelService)
    {
        _holidayService = modelService;
    }

    /// <inheritdoc />
    [HttpPost]
    public Task Import([Required] IFormFile file, [Required] HolidayType type, CancellationToken cancellationToken = default)
        => _holidayService.Import(file, type, cancellationToken);
}