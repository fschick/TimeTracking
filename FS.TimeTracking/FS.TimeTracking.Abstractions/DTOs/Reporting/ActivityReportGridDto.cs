using FS.TimeTracking.Abstractions.DTOs.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.DTOs.Reporting;

/// <summary>
/// Activity report overview DTO.
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record ActivityReportGridDto
{
    /// <inheritdoc cref="CustomerDto.Id"/>
    [Required]
    public Guid CustomerId { get; init; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string CustomerTitle { get; init; }

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