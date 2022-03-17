using System;
using System.Collections.Generic;

namespace FS.TimeTracking.Abstractions.DTOs.Shared;

/// <summary>
/// Workdays.
/// </summary>
public class WorkdaysDto
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