using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="Customer"/>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record CustomerGridDto
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

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}