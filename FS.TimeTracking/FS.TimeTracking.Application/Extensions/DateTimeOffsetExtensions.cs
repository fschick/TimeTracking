using FS.FilterExpressionCreator.Abstractions.Models;
using System;

namespace FS.TimeTracking.Application.Extensions
{
    internal static class DateTimeOffsetExtensions
    {
        public static Range<DateTimeOffset> CreateRange(this DateTimeOffset? from, DateTimeOffset? to)
            => from != null && to != null ? new(from.Value, to.Value) : null;
    }
}
