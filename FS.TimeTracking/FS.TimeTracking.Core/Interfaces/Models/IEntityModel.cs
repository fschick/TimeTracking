using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Core.Interfaces.Models;

/// <summary>
/// Entity model
/// </summary>
public interface IEntityModel
{
    /// <summary>
    /// Creation time, in coordinated universal time (UTC), of the current item.
    /// </summary>
    [Required]
    public DateTime Created { get; set; }

    /// <summary>
    /// The time, in coordinated universal time (UTC), when the current item was last written to.
    /// </summary>
    [Required]
    public DateTime Modified { get; set; }
}