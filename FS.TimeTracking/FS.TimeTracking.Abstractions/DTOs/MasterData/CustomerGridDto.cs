using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="CustomerDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record CustomerGridDto
{
    /// <inheritdoc cref="CustomerDto.Id"/>
    [Required]
    public Guid Id { get; init; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string Title { get; init; }

    /// <inheritdoc cref="CustomerDto.CompanyName"/>
    public string CompanyName { get; init; }

    /// <inheritdoc cref="CustomerDto.ContactName"/>
    public string ContactName { get; init; }

    /// <inheritdoc cref="CustomerDto.Hidden"/>
    [Required]
    public bool Hidden { get; init; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}