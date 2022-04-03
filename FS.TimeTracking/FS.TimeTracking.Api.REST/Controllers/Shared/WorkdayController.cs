using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Api.REST.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <inheritdoc cref="IWorkdayService" />
/// <seealso cref="Controller" />
/// <seealso cref="IWorkdayService" />
[V1ApiController]
public class WorkdayController : Controller, IWorkdayService
{
    private readonly IWorkdayService _workdayService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkdayController"/> class.
    /// </summary>
    /// <param name="workdayService">The workday service.</param>
    public WorkdayController(IWorkdayService workdayService)
        => _workdayService = workdayService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<WorkedDaysInfoDto> GetWorkedDaysInfo([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _workdayService.GetWorkedDaysInfo(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => await _workdayService.GetWorkdays(startDate, endDate, cancellationToken);

    /// <inheritdoc />
    [NonAction]
    Task<WorkdaysDto> IWorkdayService.GetWorkdays(Range<DateTimeOffset> dateTimeRange, CancellationToken cancellationToken)
        => throw new NotImplementedException("For internal usage only");
}