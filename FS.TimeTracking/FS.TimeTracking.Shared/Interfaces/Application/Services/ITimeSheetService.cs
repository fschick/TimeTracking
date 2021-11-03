using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services
{
    /// <inheritdoc />
    public interface ITimeSheetService : ICrudModelService<TimeSheetDto, TimeSheetListDto>
    {
        /// <summary>
        /// Get filtered data for timesheet overview.
        /// </summary>
        /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
        /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
        /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
        /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
        /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<List<TimeSheetListDto>> ListFiltered(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, CancellationToken cancellationToken = default);

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
    }
}
