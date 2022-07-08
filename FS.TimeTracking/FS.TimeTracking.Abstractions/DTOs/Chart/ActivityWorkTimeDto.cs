using FS.TimeTracking.Abstractions.DTOs.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
public class ActivityWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="ActivityDto.Id"/>
    [Required]
    public Guid ActivityId { get; set; }

    /// <inheritdoc cref="ActivityDto.Title"/>
    [Required]
    public string ActivityTitle { get; set; }
}