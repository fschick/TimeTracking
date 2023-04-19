using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="ProjectDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record ProjectGridDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    /// <inheritdoc cref="ProjectDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="ProjectDto.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="CustomerDto.Id" />
    [JsonIgnore]
    public Guid? CustomerId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="ProjectDto.Hidden"/>
    public bool Hidden { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(CustomerTitle != null ? $"({CustomerTitle})" : string.Empty)}";
}