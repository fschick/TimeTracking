using FS.TimeTracking.Abstractions.DTOs.Administration;

namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// Interface for user related grid DTOs.
/// </summary>
public interface IUserRelatedGridDto : IUserRelatedDto
{
    /// <inheritdoc cref="UserDto.Username"/>
    string Username { get; set; }
}