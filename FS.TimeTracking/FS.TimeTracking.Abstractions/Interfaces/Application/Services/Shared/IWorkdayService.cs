using FS.FilterExpressionCreator.Abstractions.Models;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;

/// <summary>
/// Workday services
/// </summary>
public interface IWorkdayService
{
    /// <summary>
    /// Gets the workdays for a given date/time range.
    /// </summary>
    /// <param name="startDate">The start date to get the workdays for.</param>
    /// <param name="endDate">The end date to get the workdays for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Enumerable with one entry per per working day.</returns>
    Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the workdays for a given date/time span.
    /// </summary>
    /// <param name="dateTimeSection">The date time section to get the workdays for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Enumerable with one entry per per working day.</returns>
    Task<WorkdaysDto> GetWorkdays(Range<DateTimeOffset> dateTimeSection, CancellationToken cancellationToken = default);
}