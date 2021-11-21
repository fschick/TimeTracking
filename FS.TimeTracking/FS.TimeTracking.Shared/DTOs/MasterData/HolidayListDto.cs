using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.MasterData;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

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