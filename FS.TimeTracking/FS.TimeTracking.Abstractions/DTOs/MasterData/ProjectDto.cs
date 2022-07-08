using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Project
/// </summary>
[ValidationDescription]
[FilterEntity(Prefix = "Project")]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ProjectDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the project.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="CustomerDto"/>.
    /// </summary>
    [Required]
    [Filter(Visible = false)]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Indicates whether this item is hidden.
    /// </summary>
    [Required]
    public bool Hidden { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}