using System;

namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// Interface for user related DTOs.
/// </summary>
public interface IUserRelatedDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    Guid UserId { get; set; }
}