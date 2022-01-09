using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FS.TimeTracking.Shared.Interfaces.Models;
using Newtonsoft.Json;

namespace FS.TimeTracking.Shared.Models.Application.MasterData;

/// <summary>
/// Activity
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class Activity : IIdEntityModel
{
    /// <inheritdoc />
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="MasterData.Project"/>.
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <inheritdoc cref="MasterData.Project"/>
    public Project Project { get; set; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Indicates whether this item is hidden.
    /// </summary>
    [Required]
    public bool Hidden { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(Project?.Customer != null ? $"({Project.Customer.Title})" : string.Empty)}";
}