using FS.TimeTracking.Abstractions.Interfaces.DTOs;

namespace FS.TimeTracking.Core.Interfaces.Models;

/// <summary>
/// Entity linked to an user via <see cref="IUserLinkedDto.UserId"/>.
/// </summary>
public interface IUserLinkedModel : IUserLinkedDto
{
}