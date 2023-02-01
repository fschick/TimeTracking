using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;

/// <inheritdoc />
public interface ITimeSheetApiService : ICrudModelService<Guid, TimeSheetDto, TimeSheetGridDto>
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
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<WorkedDaysInfoDto> GetWorkedDaysOverview(TimeSheetFilterSet filters, CancellationToken cancellationToken = default);
}