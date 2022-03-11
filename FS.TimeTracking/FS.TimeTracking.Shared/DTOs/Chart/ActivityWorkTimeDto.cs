using FS.TimeTracking.Shared.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.Chart;

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