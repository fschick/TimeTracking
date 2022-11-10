using AutoMapper;
using FS.TimeTracking.Core.Extensions;
using System;

namespace FS.TimeTracking.Application.AutoMapper;

internal class TruncateDateTimeToDayConverter : IValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffset Convert(DateTimeOffset sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerDay);
}

internal class TruncateNullableDateTimeToDayConverter : IValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public DateTimeOffset? Convert(DateTimeOffset? sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerDay);
}