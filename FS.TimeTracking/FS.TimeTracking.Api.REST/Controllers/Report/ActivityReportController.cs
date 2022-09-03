using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.Report;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Report;
using FS.TimeTracking.Report.Client.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Report;

/// <summary>
/// A controller for handling time sheet reports.
/// </summary>
[V1ApiController]
public class ActivityReportController : ControllerBase, IActivityReportService
{
    private readonly IActivityReportService _activityReportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportController"/> class.
    /// </summary>
    /// <param name="activityReportService">The activity report service.</param>
    public ActivityReportController(IActivityReportService activityReportService)
        => _activityReportService = activityReportService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<ActivityReportGridDto>> GetCustomersHavingTimeSheets([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetCustomersHavingTimeSheets(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, cancellationToken);


    /// <inheritdoc />
    [HttpGet]
    public async Task<FileResult> GetActivityReport([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReport(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, reportType, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<ReportPreviewDto> GetActivityReportPreview([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReportPreview(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, reportType, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<ActivityReportDto> GetActivityReportData([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReportData(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, reportType, cancellationToken);
}