using FS.TimeTracking.Abstractions.DTOs.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
[ExcludeFromCodeCoverage]
public record CustomerWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="CustomerDto.Id"/>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="CustomerDto.Hidden"/>
    [Required]
    public bool CustomerHidden { get; set; }
}