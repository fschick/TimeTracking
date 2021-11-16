using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.MasterData;

/// <summary>
/// Settings
/// </summary>
public class Setting : IEntityModel
{
    /// <summary>
    /// The key of this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Key { get; set; }

    /// <summary>
    /// The value of this item.
    /// </summary>
    [Required]
    public string Value { get; set; }

    /// <summary>
    /// The description of this item.
    /// </summary>
    [StringLength(100)]
    public string Description { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }
}