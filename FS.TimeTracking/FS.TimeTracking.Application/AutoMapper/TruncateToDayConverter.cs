using AutoMapper;
using FS.TimeTracking.Abstractions.Extensions;
using System;

namespace FS.TimeTracking.Application.AutoMapper;

internal class TruncateToDayConverter : IValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffset Convert(DateTimeOffset sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerDay);
}

internal class TruncateNullableToDayConverter : IValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public DateTimeOffset? Convert(DateTimeOffset? sourceMember, ResolutionContext context)
        => sourceMember.Truncate(TimeSpan.TicksPerDay);
}