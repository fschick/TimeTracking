using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Core.Models.Filter;
using Plainquire.Filter.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Workday services
/// </summary>
public interface IWorkdayService
{
    /// <summary>
    /// Gets the workdays for a given date/time span.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="dateTimeRange">The date time range to get the workdays for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Enumerable with one entry per per working day.</returns>
    Task<WorkdaysDto> GetWorkdays(TimeSheetFilterSet filters, Range<DateTimeOffset> dateTimeRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the workdays for a given date/time range.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="startDate">The start date to get the workdays for.</param>
    /// <param name="endDate">The end date to get the workdays for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Enumerable with one entry per per working day.</returns>
    Task<WorkdaysDto> GetWorkdays(TimeSheetFilterSet filters, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}