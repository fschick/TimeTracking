using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Activity
/// </summary>
[ValidationDescription]
[FilterEntity(Prefix = "Activity")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ActivityDto : IIdEntityDto
{
    ///  <summary>
    /// The display name for this item.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The title for this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="CustomerDto"/>.
    /// </summary>
    [Filter(Visible = false)]
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="ProjectDto"/>.
    /// </summary>
    [Filter(Visible = false)]
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// ID of the project's customer. Unused while create/update the entity.
    /// </summary>
    [Filter(Visible = false)]
    public Guid? ProjectCustomerId { get; set; }

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