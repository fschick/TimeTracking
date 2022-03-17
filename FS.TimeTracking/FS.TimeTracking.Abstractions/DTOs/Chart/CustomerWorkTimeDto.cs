using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for an entity.
/// </summary>
public class CustomerWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}