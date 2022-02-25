using FS.TimeTracking.Shared.DTOs.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.Shared;

/// <summary>
/// Worked time info like days.
/// </summary>
public class WorkedTimeInfoDto
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
    public TimeSpan WorkedTime { get; set; }
}