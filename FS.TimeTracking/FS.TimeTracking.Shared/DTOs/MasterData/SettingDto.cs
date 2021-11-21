using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.MasterData;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

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
}