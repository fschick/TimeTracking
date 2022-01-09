using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.DTOs.Shared;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.Application.MasterData;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;

/// <summary>
/// Service to get sources for typeahead fields
/// </summary>
public interface ITypeaheadService
{
    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Customer.Title"/>.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<string>>> GetCustomers(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Project.Title"/>.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<string>>> GetProjects(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Order.Title"/>.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<string>>> GetOrders(bool showHidden, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values for typeahead displaying <see cref="Activity.Title" />.
    /// </summary>
    /// <param name="showHidden">If set to <c>true</c>, objects marked as hidden will also be returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TypeaheadDto<string>>> GetActivities(bool showHidden, CancellationToken cancellationToken = default);
}