using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Core.Interfaces.Models;

/// <summary>
/// Entities having a property <see cref="Id"/>.
/// </summary>
public interface IIdEntityModel : IEntityModel
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    Guid Id { get; set; }
}