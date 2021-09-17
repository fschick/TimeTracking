﻿using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services
{
    /// <summary>
    /// Service to get sources for typeahead fields
    /// </summary>
    /// <autogeneratedoc />
    public interface ITypeaheadService
    {
        /// <summary>
        /// Gets the values for typeahead displaying <see cref="Customer.Title"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<List<TypeaheadDto<string>>> GetCustomers(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the values for typeahead displaying <see cref="Project.Title"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<List<TypeaheadDto<string>>> GetProjects(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the values for typeahead displaying <see cref="Order.Title"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<List<TypeaheadDto<string>>> GetOrders(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the values for typeahead displaying <see cref="Order.Number"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<List<TypeaheadDto<string>>> GetOrderNumbers(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the values for typeahead displaying <see cref="Activity.Title"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<List<TypeaheadDto<string>>> GetActivities(CancellationToken cancellationToken = default);
    }
}