using System;

namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// Entity linked to an user via <see cref="IUserLinkedDto.UserId"/>.
/// </summary>
public interface IUserLinkedDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    Guid UserId { get; set; }
}