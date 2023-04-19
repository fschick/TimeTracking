using FS.TimeTracking.Abstractions.Interfaces.DTOs;

namespace FS.TimeTracking.Core.Interfaces.Models;

/// <summary>
/// Entity linked to a customer via <see cref="ICustomerLinkedDto.CustomerId"/>.
/// </summary>
public interface ICustomerLinkedModel : ICustomerLinkedDto
{
}