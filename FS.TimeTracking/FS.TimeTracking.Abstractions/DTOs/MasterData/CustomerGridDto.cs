using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="CustomerDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record CustomerGridDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    /// <inheritdoc cref="CustomerDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Id"/>
    [JsonIgnore]
    Guid? ICustomerLinkedDto.CustomerId => Id;

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="CustomerDto.CompanyName"/>
    public string CompanyName { get; set; }

    /// <inheritdoc cref="CustomerDto.ContactName"/>
    public string ContactName { get; set; }

    /// <inheritdoc cref="CustomerDto.Hidden"/>
    [Required]
    public bool Hidden { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}