using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

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