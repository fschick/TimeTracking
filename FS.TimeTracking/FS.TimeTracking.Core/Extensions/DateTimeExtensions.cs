using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Core.Extensions;

/// <summary>
/// Extensions methods for type <see cref="DateTime"></see>
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="DateTimeOffset"/> using a given offset.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="offset">The offset to use.</param>
    public static DateTimeOffset ToOffset(this DateTime dateTime, TimeSpan offset)
        => new DateTimeOffset(dateTime, offset);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="DateTimeOffset"/> using a given offset.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="offset">The offset to use.</param>
    public static DateTimeOffset? ToOffset(this DateTime? dateTime, TimeSpan offset)
        => dateTime?.ToOffset(offset);

    /// <summary>
    /// Truncates a date to a specific precision
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    /// <param name="roundTicks">Use <see cref="TimeSpan.TicksPerMinute"/> to truncate to full minute, <see cref="TimeSpan.TicksPerHour"/> to truncate to full hour, etc.</param>
    public static DateTimeOffset Truncate(this DateTimeOffset date, long roundTicks)
        => new(date.Ticks - date.Ticks % roundTicks, date.Offset);

    /// <summary>
    /// Truncates a date to a specific precision
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    /// <param name="roundTicks">Use <see cref="TimeSpan.TicksPerMinute"/> to truncate to full minute, <see cref="TimeSpan.TicksPerHour"/> to truncate to full hour, etc.</param>
    public static DateTimeOffset? Truncate(this DateTimeOffset? date, long roundTicks)
        => date?.Truncate(roundTicks);

    /// <summary>
    /// Gets all dates between two dates.
    /// </summary>
    /// <param name="from">Start date.</param>
    /// <param name="to">End date.</param>
    public static IEnumerable<DateTime> GetDays(this DateTime from, DateTime to)
        => Enumerable
            .Range(0, (int)Math.Abs(Math.Round((to - from).TotalDays + 1, MidpointRounding.AwayFromZero)))
            .Select(x => from.AddDays(x));

    /// <summary>
    /// Gets a series of dates starting from given date.
    /// </summary>
    /// <param name="from">Start date.</param>
    /// <param name="days">The count of days to get dates for.</param>
    public static IEnumerable<DateTime> GetDays(this DateTime from, int days)
        => Enumerable
            .Range(0, days)
            .Select(x => from.AddDays(x));

    /// <summary>
    /// Gets all dates of a month.
    /// </summary>
    /// <param name="dateTime">The year and month to get the days for.</param>
    public static IEnumerable<DateTime> GetDaysOfMonth(this DateTime dateTime)
        => Enumerable
            .Range(1, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
            .Select(day => new DateTime(dateTime.Year, dateTime.Month, day));

    /// <summary>
    /// Gets all dates of a month till a given date.
    /// </summary>
    /// <param name="dateTime">The date till the days should be returned.</param>
    public static IEnumerable<DateTime> GetDaysOfMonthTillDay(this DateTime dateTime)
        => Enumerable
            .Range(1, dateTime.Day)
            .Select(day => new DateTime(dateTime.Year, dateTime.Month, day));

    /// <summary>
    /// Converts a <see cref="DateTime"/> to UTC time using a specific offset.
    /// </summary>
    /// <param name="dateTime">The date time expressed as UTC.</param>
    /// <param name="offset">The offset in minutes to remove.</param>
    public static DateTime ToUtc(this DateTime dateTime, int offset)
        => dateTime.AddMinutes(offset * -1);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to UTC time using a specific offset.
    /// </summary>
    /// <param name="dateTime">The date time expressed as UTC.</param>
    /// <param name="offset">The offset in minutes to remove.</param>
    public static DateTime? ToUtc(this DateTime? dateTime, int offset)
        => dateTime.HasValue ? ToUtc(dateTime.Value, offset) : null;

    /// <summary>
    /// Converts a <see cref="DateTime"/> to <see cref="DateTimeOffset"/> using a given time zone.
    /// </summary>
    /// <param name="dateTime">The date/time to convert.</param>
    /// <param name="timeZone">The time zone to convert the date to.</param>
    public static DateTimeOffset ConvertTo(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        var convertedDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        var offset = timeZone.GetUtcOffset(convertedDateTime);
        return new DateTimeOffset(convertedDateTime, offset);
    }

    /// <summary>
    /// Calculates the difference of two dates in seconds.
    /// </summary>
    /// <param name="fromDate">Start date.</param>
    /// <param name="fromOffset">The offset of start date from UTC in minutes</param>
    /// <param name="toDate">End date.</param>
    /// <param name="toOffset">The offset of to date from UTC in minutes</param>
    public static ulong DiffSeconds(this DateTime fromDate, int fromOffset, DateTime? toDate, int? toOffset)
    {
        var from = new DateTimeOffset(fromDate, TimeSpan.FromMinutes(fromOffset));
        var to = toDate.HasValue
            ? new DateTimeOffset(toDate.Value, TimeSpan.FromMinutes(toOffset ?? 0))
            : DateTimeOffset.UtcNow;
        return (ulong)(to - from).TotalSeconds;
    }

    /// <summary>
    /// Gets the start date of the day.
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    public static DateTime StartOfDay(this DateTime date)
        => date.Date;

    /// <summary>
    /// Gets the start date of the week.
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    /// <param name="startOfWeek">The start of week. Defaults to monday (ISO 8601)</param>
    public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the start date of the month.
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    public static DateTime StartOfMonth(this DateTime date)
        => date.AddDays(-date.Day + 1).Date;

    /// <summary>
    /// Gets the start date of the year.
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    public static DateTime StartOfYear(this DateTime date)
        => date.AddDays(-date.DayOfYear + 1).Date;

    /// <summary>
    /// Gets the start date of the year.
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    public static DateTimeOffset StartOfYear(this DateTimeOffset date)
        => date.AddDays(-date.DayOfYear + 1).Date;

    /// <summary>
    /// Gets the start date of the year.
    /// </summary>
    /// <param name="date">The date to truncate.</param>
    public static DateTimeOffset EndOfYear(this DateTimeOffset date)
        => date.StartOfYear().AddYears(1).AddTicks(-1);
}