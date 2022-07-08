using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="ActivityDto"/>
[System.Diagnostics.DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ActivityGridDto
{
    /// <inheritdoc cref="ActivityDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="ActivityDto.Title"/>
    public string Title { get; set; }

    /// <inheritdoc cref="ProjectDto.Title"/>
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="ProjectDto.Hidden"/>
    public bool Hidden { get; set; }

    [JsonIgnore]
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(CustomerTitle != null ? $"({CustomerTitle})" : string.Empty)}";
}