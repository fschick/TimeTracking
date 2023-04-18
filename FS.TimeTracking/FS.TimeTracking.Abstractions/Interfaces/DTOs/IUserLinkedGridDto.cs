using FS.TimeTracking.Abstractions.DTOs.Administration;

namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// Entity linked to an user via <see cref="IUserLinkedDto.UserId"/>.
/// </summary>
public interface IUserLinkedGridDto : IUserLinkedDto
{
    /// <inheritdoc cref="UserDto.Username"/>
    string Username { get; set; }
}