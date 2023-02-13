using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// Interface to identify DTOs with ID.
/// </summary>
public interface IIdEntityDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    Guid Id { get; set; }
}