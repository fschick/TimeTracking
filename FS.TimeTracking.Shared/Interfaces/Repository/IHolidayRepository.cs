using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Repository
{
    /// <summary>
    /// Bank and school holiday repository.
    /// </summary>
    public interface IHolidayRepository
    {
        /// <summary>
        /// Gets all bank holidays for a given year.
        /// </summary>
        /// <param name="year">The year to get the bank holidays for.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns>Enumerable with one entry per day for all holidays.</returns>
        Task<List<DateTime>> GetHolidays(int year, CancellationToken cancellationToken = default);
    }
}