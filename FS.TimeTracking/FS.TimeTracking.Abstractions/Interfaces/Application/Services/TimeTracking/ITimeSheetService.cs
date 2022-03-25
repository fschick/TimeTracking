using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;
using System;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.TimeTracking;

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
}