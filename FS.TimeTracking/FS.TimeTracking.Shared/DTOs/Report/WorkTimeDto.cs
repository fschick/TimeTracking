using FS.TimeTracking.Shared.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.Report;

/// <summary>
/// Work times for an entity.
/// </summary>
public abstract class WorkTimeDto
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
    public double? DaysDifference => DaysPlanned - DaysWorked;

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
    public TimeSpan? TimeDifference => TimePlanned - TimeWorked;

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
    public double? BudgetDifference => BudgetPlanned - BudgetWorked;

    /// <summary>
    /// Ratio between worked and planned days/time/budget.
    /// </summary>
    [Required]
    public double? PercentDifference => DaysPlanned != null ? 1 - (DaysPlanned != 0 ? DaysWorked / DaysPlanned : 1) : null;

    /// <summary>
    /// Ratio of worked time related to sibling <see cref="WorkTimeDto"/>.
    /// </summary>
    [Required]
    public double RatioTotalWorked { get; set; }

    /// <summary>
    /// Ratio of time planned related to sibling <see cref="WorkTimeDto"/>.
    /// </summary>
    [Required]
    public double? RatioTotalPlanned { get; set; }

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

    /// <inheritdoc cref="Order.HourlyRate"/>
    [Required]
    public double? PlannedHourlyRate { get; set; }

    /// <summary>
    /// The currency of the order.
    /// </summary>
    [Required]
    public string Currency { get; set; }
}