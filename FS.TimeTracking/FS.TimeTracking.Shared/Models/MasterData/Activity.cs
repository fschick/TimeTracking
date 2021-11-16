using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Interfaces.Models;

namespace FS.TimeTracking.Shared.Models.MasterData;

/// <summary>
/// Activity
/// </summary>
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
    /// Identifier to the related <see cref="Project"/>.
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
}