using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.Chart;

/// <summary>
/// Work times.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class WorkTime
{
    /// <inheritdoc cref="WorkTimeDto.TimeWorked"/>
    [Required]
    public TimeSpan WorkedTime { get; set; }

    /// <inheritdoc cref="WorkTimeDto.DaysWorked"/>
    [Required]
    public double WorkedDays { get; set; }

    /// <inheritdoc cref="WorkTimeDto.BudgetWorked"/>
    [Required]
    public double WorkedBudget { get; set; }

    /// <inheritdoc cref="WorkTimeDto.TimePlanned"/>
    [Required]
    public TimeSpan PlannedTime { get; set; }

    /// <inheritdoc cref="WorkTimeDto.DaysPlanned"/>
    [Required]
    public double PlannedDays { get; set; }

    /// <inheritdoc cref="WorkTimeDto.BudgetPlanned"/>
    [Required]
    public double PlannedBudget { get; set; }

    /// <inheritdoc cref="Order.StartDate"/>
    [Required]
    public DateTimeOffset? PlannedStart { get; set; }

    /// <inheritdoc cref="Order.DueDate"/>
    [Required]
    public DateTimeOffset? PlannedEnd { get; set; }
}