using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.TimeTracking;

/// <inheritdoc cref="ITimeSheetService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ITimeSheetService" />
[V1ApiController]
public class TimeSheetController : CrudModelController<TimeSheetDto, TimeSheetGridDto>, ITimeSheetService
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
    public async Task<WorkedDaysInfoDto> GetWorkedDaysOverview([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _timeSheetService.GetWorkedDaysOverview(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);
}