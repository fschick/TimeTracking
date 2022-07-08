using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Activity
/// </summary>
[ValidationDescription]
[FilterEntity(Prefix = "Activity")]
[System.Diagnostics.DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ActivityDto
{
    ///  <summary>
    /// The display name of the activity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="ActivityDto.Title"/>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="ProjectDto"/>.
    /// </summary>
    [Filter(Visible = false)]
    public Guid? ProjectId { get; set; }

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
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}