using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="Project"/>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ProjectListDto
{
    /// <inheritdoc cref="Project.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Project.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="Project.Hidden"/>
    public bool Hidden { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(CustomerTitle != null ? $"({CustomerTitle})" : string.Empty)}";
}