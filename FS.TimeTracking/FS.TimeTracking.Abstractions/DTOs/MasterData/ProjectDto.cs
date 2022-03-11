using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Shared.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="Project"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Project))]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ProjectDto
{
    /// <inheritdoc cref="Project.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Project.Title"/>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <inheritdoc cref="Project.CustomerId"/>
    [Required]
    [Filter(Visible = false)]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="Project.Comment"/>
    public string Comment { get; set; }

    /// <inheritdoc cref="Project.Hidden"/>
    [Required]
    public bool Hidden { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}