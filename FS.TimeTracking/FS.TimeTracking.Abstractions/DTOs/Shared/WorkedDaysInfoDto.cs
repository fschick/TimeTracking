using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Shared;

/// <summary>
/// Worked time info like days.
/// </summary>
[ExcludeFromCodeCoverage]
public record WorkedDaysInfoDto
{
    /// <summary>
    /// Count of public workdays.
    /// </summary>
    [Required]
    public int PublicWorkdays { get; set; }

    /// <summary>
    /// Count of public workdays except individual holidays.
    /// </summary>
    [Required]
    public int PersonalWorkdays { get; set; }

    /// <summary>
    /// Count of individual days of holiday.
    /// </summary>
    [Required]
    public int PersonalHolidays => PublicWorkdays - PersonalWorkdays;

    /// <inheritdoc cref="SettingDto.WorkHoursPerWorkday" />
    [Required]
    public TimeSpan WorkHoursPerWorkday { get; set; }

    /// <summary>
    /// Worked time.
    /// </summary>
    [Required]
    public TimeSpan TotalTimeWorked { get; set; }

    /// <summary>
    /// Last worked times per day/week/month/year.
    /// </summary>
    [Required]
    public List<WorkdayDto> LastWorkedTimes { get; set; }

    /// <summary>
    /// Aggregation unit for <see cref="LastWorkedTimes"/>.
    /// </summary>
    [Required]
    public WorkdayAggregationUnit LastWorkedTimesAggregationUnit { get; set; }
}