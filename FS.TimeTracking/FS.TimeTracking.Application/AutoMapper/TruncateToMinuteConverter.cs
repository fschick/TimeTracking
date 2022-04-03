using AutoMapper;
using FS.TimeTracking.Shared.Extensions;
using System;

namespace FS.TimeTracking.Application.AutoMapper;

internal class TruncateToMinuteConverter : IValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffset Convert(DateTimeOffset sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerMinute);
}

internal class TruncateNullableToMinuteConverter : IValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public DateTimeOffset? Convert(DateTimeOffset? sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerMinute);
}