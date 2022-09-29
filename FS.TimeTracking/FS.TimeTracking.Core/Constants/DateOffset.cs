using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Constants;

/// <summary>
/// Timezone conversion safe constants for <see cref="DateTimeOffset"/>
/// </summary>
[ExcludeFromCodeCoverage]
public static class DateOffset
{
    /// <summary>
    /// Earliest timezone conversion safe <see cref="DateTimeOffset.MinValue"/>
    /// </summary>
    public static readonly DateTimeOffset MinDate = DateTimeOffset.MinValue.AddDays(2); // Let space for timezone conversions.

    /// <summary>
    /// Greatest timezone conversion safe <see cref="DateTimeOffset.MaxValue"/>
    /// </summary>
    public static readonly DateTimeOffset MaxDate = DateTimeOffset.MaxValue.AddDays(-2); // Let space for timezone conversions.
}