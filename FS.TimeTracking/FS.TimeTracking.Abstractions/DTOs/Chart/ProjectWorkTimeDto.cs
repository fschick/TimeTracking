using FS.TimeTracking.Abstractions.DTOs.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for a project.
/// </summary>
[ExcludeFromCodeCoverage]
public record ProjectWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="CustomerDto.Id"/>
    [Required]
    public Guid ProjectId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public bool ProjectHidden { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}