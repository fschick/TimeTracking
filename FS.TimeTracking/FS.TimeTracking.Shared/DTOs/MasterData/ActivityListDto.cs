using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.Application.MasterData;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Project"/>
[System.Diagnostics.DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ActivityListDto
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