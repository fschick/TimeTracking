using FS.TimeTracking.Abstractions.DTOs.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
public class OrderWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="OrderDto.Id" />
    [Required]
    public Guid OrderId { get; set; }

    /// <inheritdoc cref="OrderDto.Title" />
    [Required]
    public string OrderTitle { get; set; }

    /// <inheritdoc cref="OrderDto.Number" />
    [Required]
    public string OrderNumber { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}