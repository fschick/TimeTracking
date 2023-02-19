using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract record WorkTimeDto
{
    /// <summary>
    /// Time worked in work days.
    /// </summary>
    [Required]
    public double DaysWorked { get; set; }

    /// <summary>
    /// Time planned in work days.
    /// </summary>
    [Required]
    public double? DaysPlanned { get; set; }

    /// <summary>
    /// Difference between worked and planned days.
    /// </summary>
    [Required]
    public double? DaysDifference { get; set; }

    /// <summary>
    /// Time worked.
    /// </summary>
    [Required]
    public TimeSpan TimeWorked { get; set; }

    /// <summary>
    /// Time planned.
    /// </summary>
    [Required]
    public TimeSpan? TimePlanned { get; set; }

    /// <summary>
    /// Difference between time worked and planned.
    /// </summary>
    [Required]
    public TimeSpan? TimeDifference { get; set; }

    /// <summary>
    /// Consumed budget.
    /// </summary>
    [Required]
    public double BudgetWorked { get; set; }

    /// <summary>
    /// Planned budget.
    /// </summary>
    [Required]
    public double? BudgetPlanned { get; set; }

    /// <summary>
    /// Difference between consumed and planned budget.
    /// </summary>
    [Required]
    public double? BudgetDifference { get; set; }

    /// <summary>
    /// Start date of planned time.
    /// </summary>
    public DateTimeOffset? PlannedStart { get; set; }

    /// <summary>
    /// End date of planned time.
    /// </summary>
    public DateTimeOffset? PlannedEnd { get; set; }

    /// <summary>
    /// Indicates whether order period is only partially matched by selected period.
    /// </summary>
    [Required]
    public bool PlannedIsPartial { get; set; }

    /// <summary>
    /// Ratio of worked time related to all other <see cref="WorkTimeDto"/>.
    /// </summary>
    [Required]
    public double TotalWorkedPercentage { get; set; }

    /// <summary>
    /// Ratio of time planned related to all other <see cref="WorkTimeDto"/>.
    /// </summary>
    [Required]
    public double? TotalPlannedPercentage { get; set; }

    /// <summary>
    /// The currency of the order.
    /// </summary>
    [Required]
    public string Currency { get; set; }
}