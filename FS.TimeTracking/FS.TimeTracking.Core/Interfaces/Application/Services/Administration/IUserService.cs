using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using FS.TimeTracking.Core.Models.Filter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Administration;

/// <inheritdoc />
public interface IUserService : IUserApiService
{
    /// <summary>
    /// Sets user related fields of a DTO.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="dto">The DTO to work on.</param>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    Task SetUserRelatedProperties<T>(T dto, CancellationToken cancellationToken) where T : class, IUserLinkedGridDto;

    /// <summary>
    /// Sets user related fields of a DTO.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="filters">The filters.</param>
    /// <param name="dtos">The DTOs to work on.</param>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    Task SetUserRelatedProperties<T>(TimeSheetFilterSet filters, List<T> dtos, CancellationToken cancellationToken) where T : class, IUserLinkedGridDto;

    /// <summary>
    /// Gets items filtered.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<UserDto>> GetFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default);
}