using AutoMapper;
using FS.TimeTracking.Core.Extensions;
using System;

namespace FS.TimeTracking.Application.AutoMapper;

internal class TruncateDateTimeToMinuteConverter : IValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffset Convert(DateTimeOffset sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerMinute);
}

internal class TruncateNullableDateTimeToMinuteConverter : IValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public DateTimeOffset? Convert(DateTimeOffset? sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerMinute);
}