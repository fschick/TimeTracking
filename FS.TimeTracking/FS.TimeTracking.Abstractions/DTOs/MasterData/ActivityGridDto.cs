using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="ActivityDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record ActivityGridDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    /// <inheritdoc cref="ActivityDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="ActivityDto.Title"/>
    public string Title { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public Guid? CustomerId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="ProjectDto.Title"/>
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="ProjectDto.Hidden"/>
    public bool Hidden { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(CustomerTitle != null ? $"({CustomerTitle})" : string.Empty)}";
}