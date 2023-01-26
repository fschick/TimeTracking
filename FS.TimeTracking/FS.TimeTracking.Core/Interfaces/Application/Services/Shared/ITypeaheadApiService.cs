using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Service to get sources for typeahead fields
/// </summary>
public interface ITypeaheadApiService
{
    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Customer.Title"/>.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<Guid, string>>> GetCustomers(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Project.Title"/>.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<Guid, string>>> GetProjects(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Order.Title"/>.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<Guid, string>>> GetOrders(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Activity.Title" />.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<Guid, string>>> GetActivities(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="UserDto.Username" />.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<Guid, string>>> GetUsers(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available timezones.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<string, string>>> GetTimezones(CancellationToken cancellationToken = default);
}