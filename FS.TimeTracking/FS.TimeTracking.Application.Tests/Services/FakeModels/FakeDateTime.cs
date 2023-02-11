using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeDateTime
{
    public readonly TimeZoneInfo DefaultTimezone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

    public DateTimeOffset Offset(string dateTime, TimeZoneInfo timeZone = null)
    {
        var parsedDateTime = DateTime.Parse(dateTime);
        timeZone ??= DefaultTimezone;
        return new DateTimeOffset(parsedDateTime, timeZone.GetUtcOffset(parsedDateTime));
    }
}