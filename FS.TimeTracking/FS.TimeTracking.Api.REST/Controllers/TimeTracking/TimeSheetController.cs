using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.TimeTracking;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.DTOs.MasterData;

namespace FS.TimeTracking.Api.REST.Controllers.TimeTracking;

/// <inheritdoc cref="ITimeSheetService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ITimeSheetService" />
[V1ApiController]
public class TimeSheetController : CrudModelController<TimeSheetDto, TimeSheetListDto>, ITimeSheetService
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
    [HttpGet]
    public Task<List<TimeSheetListDto>> ListFiltered([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, CancellationToken cancellationToken = default)
        => _timeSheetService.ListFiltered(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, cancellationToken);

    /// <inheritdoc />
    [HttpPut]
    public Task<TimeSheetDto> StartSimilarTimeSheetEntry(Guid copyFromTimesheetId, DateTimeOffset startDateTime)
        => _timeSheetService.StartSimilarTimeSheetEntry(copyFromTimesheetId, startDateTime);

    /// <inheritdoc />
    [HttpPut]
    public Task<TimeSheetDto> StopTimeSheetEntry(Guid timesheetId, DateTimeOffset endDateTime)
        => _timeSheetService.StopTimeSheetEntry(timesheetId, endDateTime);
}