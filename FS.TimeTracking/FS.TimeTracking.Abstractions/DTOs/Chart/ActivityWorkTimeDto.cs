using FS.TimeTracking.Abstractions.DTOs.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
[ExcludeFromCodeCoverage]
public record ActivityWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="ActivityDto.Id"/>
    [Required]
    public Guid ActivityId { get; set; }

    /// <inheritdoc cref="ActivityDto.Title"/>
    [Required]
    public string ActivityTitle { get; set; }

    /// <inheritdoc cref="ActivityDto.Hidden"/>
    [Required]
    public bool ActivityHidden { get; set; }
}