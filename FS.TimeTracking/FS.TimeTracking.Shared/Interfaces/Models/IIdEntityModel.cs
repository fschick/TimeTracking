using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Interfaces.Models;

/// <summary>
/// Interface for entity models
/// </summary>
public interface IIdEntityModel : IEntityModel
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }
}