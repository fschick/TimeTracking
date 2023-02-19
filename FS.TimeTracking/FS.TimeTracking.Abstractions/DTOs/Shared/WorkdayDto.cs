using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Shared;

/// <summary>
/// Last worked times.
/// </summary>
[ExcludeFromCodeCoverage]
public record WorkdayDto
{
    /// <summary>
    /// The date.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// The time worked.
    /// </summary>
    [Required]
    public TimeSpan TimeWorked { get; set; }
}