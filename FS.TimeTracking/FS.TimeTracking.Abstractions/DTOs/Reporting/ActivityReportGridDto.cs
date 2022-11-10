using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Reporting;

/// <summary>
/// Activity report overview DTO.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record ActivityReportGridDto
{
    /// <inheritdoc cref="CustomerDto.Id"/>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="WorkTimeDto.DaysWorked"/>
    [Required]
    public double DaysWorked { get; set; }

    /// <inheritdoc cref="WorkTimeDto.TimeWorked"/>
    [Required]
    public TimeSpan TimeWorked { get; set; }

    /// <inheritdoc cref="WorkTimeDto.BudgetWorked"/>
    [Required]
    public double BudgetWorked { get; set; }

    /// <inheritdoc cref="WorkTimeDto.Currency"/>
    [Required]
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the relative URL to the daily activity report.
    /// </summary>
    public string DailyActivityReportUrl { get; set; }

    /// <summary>
    /// Gets or sets the relative URL to the detailed activity report.
    /// </summary>
    public string DetailedActivityReportUrl { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{CustomerTitle}";
}