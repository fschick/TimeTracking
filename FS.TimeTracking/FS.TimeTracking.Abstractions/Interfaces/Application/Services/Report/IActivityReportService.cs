using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.Enums.Report;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.Report;

/// <summary>
/// Time sheet report service
/// </summary>
public interface IActivityReportService
{
    /// <summary>
    /// Gets a detailed activity report.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="groupBy">Field the report should be grouped by.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ActivityReportDto> GetDetailedActivityReport(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, string language, ActivityReportGroup groupBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the preview of a detailed activity report.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="groupBy">Field the report should be grouped by.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ActivityReportDto> GetDetailedActivityReportPreview(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, string language, ActivityReportGroup groupBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets data to create activity based reports.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="groupBy">Field the report should be grouped by.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ActivityReportDto> GetActivityReportData(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, string language, ActivityReportGroup groupBy, CancellationToken cancellationToken = default);
}