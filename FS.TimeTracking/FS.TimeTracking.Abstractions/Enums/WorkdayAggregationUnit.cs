namespace FS.TimeTracking.Abstractions.Enums;

/// <summary>
/// Work day aggregation units.
/// </summary>
public enum WorkdayAggregationUnit
{
    /// <summary>
    /// Invalid group.
    /// </summary>
    Invalid,

    /// <summary>
    /// Grouped by day.
    /// </summary>
    Day,

    /// <summary>
    /// Grouped by week.
    /// </summary>
    Week,

    /// <summary>
    /// Grouped by month.
    /// </summary>
    Month,

    /// <summary>
    /// Grouped by year.
    /// </summary>
    Year
}