using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeDateTime
{
    public static readonly TimeZoneInfo DefaultTimezone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

    public static DateTimeOffset Offset(string dateTime, TimeZoneInfo timeZone = null)
    {
        var parsedDateTime = DateTime.Parse(dateTime);
        timeZone ??= DefaultTimezone;
        return new DateTimeOffset(parsedDateTime, timeZone.GetUtcOffset(parsedDateTime));
    }
}