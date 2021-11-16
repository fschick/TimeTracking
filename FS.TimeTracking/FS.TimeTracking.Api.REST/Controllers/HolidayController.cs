using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers;

/// <inheritdoc cref="IHolidayService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IHolidayService" />
[V1ApiController]
public class HolidayController : CrudModelController<HolidayDto, HolidayListDto>, IHolidayService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HolidayController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public HolidayController(IHolidayService modelService)
        : base(modelService)
    {
    }
}