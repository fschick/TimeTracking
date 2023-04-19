using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="OrderDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record OrderGridDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    /// <inheritdoc cref="OrderDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="OrderDto.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="CustomerDto.Id" />
    [JsonIgnore]
    public Guid? CustomerId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="OrderDto.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="OrderDto.DueDate"/>
    [Required]
    public DateTimeOffset DueDate { get; set; }

    /// <inheritdoc cref="OrderDto.Hidden"/>
    public bool Hidden { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}