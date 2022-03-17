using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
public class ActivityWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="Activity.Id"/>
    [Required]
    public Guid ActivityId { get; set; }

    /// <inheritdoc cref="Activity.Title"/>
    [Required]
    public string ActivityTitle { get; set; }
}