using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking;

/// <inheritdoc cref="Setting"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Setting))]
public record SettingDto
{
    /// <summary>
    /// Working hours per day of week.
    /// </summary>
    [Required]
    public Dictionary<DayOfWeek, TimeSpan> WorkingHours { get; set; } = new()
    {
        { DayOfWeek.Monday, TimeSpan.FromHours(8) },
        { DayOfWeek.Tuesday, TimeSpan.FromHours(8) },
        { DayOfWeek.Wednesday, TimeSpan.FromHours(8) },
        { DayOfWeek.Thursday, TimeSpan.FromHours(8) },
        { DayOfWeek.Friday, TimeSpan.FromHours(8) },
        { DayOfWeek.Saturday, TimeSpan.Zero },
        { DayOfWeek.Sunday, TimeSpan.Zero },
    };
}