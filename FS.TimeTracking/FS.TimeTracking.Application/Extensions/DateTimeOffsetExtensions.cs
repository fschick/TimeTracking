using Plainquire.Filter.Abstractions;
using System;

namespace FS.TimeTracking.Application.Extensions;

internal static class DateTimeOffsetExtensions
{
    public static Range<DateTimeOffset> CreateRange(this DateTimeOffset? from, DateTimeOffset? to)
        => from != null && to != null ? new Range<DateTimeOffset>(from.Value, to.Value) : null;
}