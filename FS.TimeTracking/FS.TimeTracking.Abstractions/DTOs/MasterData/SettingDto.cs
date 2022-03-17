using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="Setting"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Setting))]
public record SettingDto
{
    /// <summary>
    /// Workdays
    /// </summary>
    [Required]
    public Dictionary<DayOfWeek, bool> Workdays { get; set; } = new()
    {
        { DayOfWeek.Monday, true },
        { DayOfWeek.Tuesday, true },
        { DayOfWeek.Wednesday, true },
        { DayOfWeek.Thursday, true },
        { DayOfWeek.Friday, true },
        { DayOfWeek.Saturday, false },
        { DayOfWeek.Sunday, false },
    };

    /// <summary>
    /// The average working hours per workday
    /// </summary>
    [Required]
    public TimeSpan WorkHoursPerWorkday = TimeSpan.FromHours(8);

    /// <summary>
    /// The currency to use
    /// </summary>
    [Required]
    public string Currency = "€";
}