using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.TimeTracking;

/// <inheritdoc cref="ITimeSheetService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ITimeSheetService" />
[V1ApiController]
[ExcludeFromCodeCoverage]
public class TimeSheetController : CrudModelController<Guid, TimeSheetDto, TimeSheetGridDto>, ITimeSheetService
{
    private readonly ITimeSheetService _timeSheetService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSheetController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public TimeSheetController(ITimeSheetService modelService)
        : base(modelService)
        => _timeSheetService = modelService;

    /// <inheritdoc />
    [HttpPut]
    public async Task<TimeSheetDto> StartSimilarTimeSheetEntry(Guid copyFromTimesheetId, DateTimeOffset startDateTime)
        => await _timeSheetService.StartSimilarTimeSheetEntry(copyFromTimesheetId, startDateTime);

    /// <inheritdoc />
    [HttpPut]
    public async Task<TimeSheetDto> StopTimeSheetEntry(Guid timesheetId, DateTimeOffset endDateTime)
        => await _timeSheetService.StopTimeSheetEntry(timesheetId, endDateTime);

    /// <inheritdoc />
    [HttpGet]
    public async Task<WorkedDaysInfoDto> GetWorkedDaysOverview([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _timeSheetService.GetWorkedDaysOverview(filters, cancellationToken);
}