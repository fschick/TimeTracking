using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;

/// <inheritdoc />
public interface ITimeSheetService : ICrudModelService<TimeSheetDto, TimeSheetGridDto>
{
    /// <summary>
    /// Starts a similar time sheet entry.
    /// </summary>
    /// <param name="copyFromTimesheetId">The timesheet identifier to copy values copy from.</param>
    /// <param name="startDateTime">The start date time.</param>
    Task<TimeSheetDto> StartSimilarTimeSheetEntry(Guid copyFromTimesheetId, DateTimeOffset startDateTime);

    /// <summary>
    /// Stops the time sheet entry.
    /// </summary>
    /// <param name="timesheetId">The timesheet identifier.</param>
    /// <param name="endDateTime">The end date time.</param>
    Task<TimeSheetDto> StopTimeSheetEntry(Guid timesheetId, DateTimeOffset endDateTime);

    /// <summary>
    /// Gets info about days worked, public and private workdays / leave.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<WorkedDaysInfoDto> GetWorkedDaysOverview(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default);
}