using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services
{
    /// <summary>
    /// CRUD model services
    /// </summary>
    /// <typeparam name="TDto">The type of the entity DTO.</typeparam>
    /// <typeparam name="TListDto">The type of the query DTO</typeparam>
    public interface ICrudModelService<TDto, TListDto>
    {
        /// <summary>
        /// Gets all items
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<List<TListDto>> List(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the item specified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<TDto> Get(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="dto">The item to create.</param>
        Task<TDto> Create(TDto dto);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dto">The item to update.</param>
        Task<TDto> Update(TDto dto);

        /// <summary>
        /// Deletes the item specified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        Task<long> Delete(Guid id);
    }
}