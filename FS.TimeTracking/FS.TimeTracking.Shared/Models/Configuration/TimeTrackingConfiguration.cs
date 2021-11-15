using System;
using System.Collections.Generic;

namespace FS.TimeTracking.Shared.Models.Configuration;

/// <summary>
/// Global application configuration
/// </summary>
public class TimeTrackingConfiguration
{
    /// <summary>
    /// The configuration section bind to.
    /// </summary>
    public const string CONFIGURATION_SECTION = "TimeTracking";

    /// <summary>
    /// Working days of a week.
    /// </summary>
    public IEnumerable<DayOfWeek> WorkingDays { get; set; }

    /// <summary>
    /// Database specific configuration.
    /// </summary>
    public DatabaseConfiguration Database { get; set; } = new();
}