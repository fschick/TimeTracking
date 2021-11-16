using FS.TimeTracking.Shared.Models.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking;

/// <inheritdoc cref="Holiday"/>
public record HolidayListDto
{
    /// <inheritdoc cref="Holiday.Id"/>
    [Required]
    public Guid Id { get; init; }

    /// <inheritdoc cref="Holiday.Title"/>
    [Required]
    public string Title { get; init; }

    /// <inheritdoc cref="Holiday.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="Holiday.EndDate"/>
    [Required]
    public DateTimeOffset EndDate { get; set; }
}