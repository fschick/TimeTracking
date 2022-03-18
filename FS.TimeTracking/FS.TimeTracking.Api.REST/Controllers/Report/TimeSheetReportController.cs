using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Report;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.Report;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Report;

/// <summary>
/// A controller for handling time sheet reports.
/// </summary>
[V1ApiController]
public class TimeSheetReportController : ControllerBase, ITimeSheetReportService
{
    private readonly ITimeSheetReportService _timeSheetReportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSheetReportController"/> class.
    /// </summary>
    /// <param name="timeSheetReportService">The time sheet report service.</param>
    public TimeSheetReportController(ITimeSheetReportService timeSheetReportService)
        => _timeSheetReportService = timeSheetReportService;


    /// <inheritdoc />
    [HttpGet]
    public async Task<TimeSheetReportDto> GetFullTimeSheetReport([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, string language, CancellationToken cancellationToken = default)
        => await _timeSheetReportService.GetFullTimeSheetReport(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<TimeSheetReportDataDto> GetTimeSheetReportData([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, string language, CancellationToken cancellationToken = default)
        => await _timeSheetReportService.GetTimeSheetReportData(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, cancellationToken);
}