using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Shared;

/// <summary>
/// Workdays.
/// </summary>
[ExcludeFromCodeCoverage]
public record WorkdaysDto
{
    /// <summary>
    /// Public workdays.
    /// </summary>
    public List<DateTime> PublicWorkdays { get; set; }

    /// <summary>
    /// Workdays taking individual leave into account.
    /// </summary>
    public List<DateTime> PersonalWorkdays { get; set; }
}