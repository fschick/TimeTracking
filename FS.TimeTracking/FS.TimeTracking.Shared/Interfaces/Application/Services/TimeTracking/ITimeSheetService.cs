using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using System;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.TimeTracking;

/// <inheritdoc />
public interface ITimeSheetService : ICrudModelService<TimeSheetDto, TimeSheetListDto>
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