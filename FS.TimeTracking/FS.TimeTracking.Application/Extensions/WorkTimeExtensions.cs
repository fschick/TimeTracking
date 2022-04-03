using FS.TimeTracking.Abstractions.Models.Application.Chart;
using FS.TimeTracking.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Application.Extensions;

internal static class WorkTimeExtensions
{
    public static double GetAverageHourlyRate(this IReadOnlyCollection<WorkTime> workTimes, Func<WorkTime, TimeSpan?> timeSelector)
    {
        var times = workTimes.Select(x => new { Hours = timeSelector(x) ?? TimeSpan.Zero, x.HourlyRate }).ToList();
        var hours = times.Sum(o => o.Hours.TotalHours);
        var budget = times.Sum(o => o.Hours.TotalHours * o.HourlyRate);
        var averageHourlyRate = hours != 0 ? budget / hours : 0;
        return averageHourlyRate;
    }
}