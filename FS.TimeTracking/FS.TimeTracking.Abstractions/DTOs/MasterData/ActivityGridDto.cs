using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="Project"/>
[System.Diagnostics.DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ActivityGridDto
{
    /// <inheritdoc cref="Activity.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Activity.Title"/>
    public string Title { get; set; }

    /// <inheritdoc cref="Project.Title"/>
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="Project.Hidden"/>
    public bool Hidden { get; set; }

    [JsonIgnore]
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(CustomerTitle != null ? $"({CustomerTitle})" : string.Empty)}";
}