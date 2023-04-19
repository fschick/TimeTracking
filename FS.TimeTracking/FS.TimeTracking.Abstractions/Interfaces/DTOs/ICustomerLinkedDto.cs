using System;

namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// DTO linked to customer.
/// </summary>
public interface ICustomerLinkedDto
{
    /// <summary>
    /// The unique identifier of the customer.
    /// </summary>
    Guid? CustomerId { get; }
}