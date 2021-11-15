﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Shared.Extensions;

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
    /// Gets all dates between two dates.
    /// </summary>
    /// <param name="from">Start date.</param>
    /// <param name="to">End date.</param>
    public static IEnumerable<DateTime> GetDays(this DateTime from, DateTime to)
        => Enumerable
            .Range(0, (int)Math.Abs(Math.Round((to - from).TotalDays, MidpointRounding.AwayFromZero)))
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
}