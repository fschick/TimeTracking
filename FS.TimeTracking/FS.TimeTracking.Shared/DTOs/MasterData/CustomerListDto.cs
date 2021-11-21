using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.MasterData;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Customer"/>
public record CustomerListDto
{
    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid Id { get; init; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string Title { get; init; }

    /// <inheritdoc cref="Customer.CompanyName"/>
    public string CompanyName { get; init; }

    /// <inheritdoc cref="Customer.ContactName"/>
    public string ContactName { get; init; }

    /// <inheritdoc cref="Customer.Hidden"/>
    [Required]
    public bool Hidden { get; init; }
}