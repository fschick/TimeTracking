using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Administration;

/// <inheritdoc />
public interface IUserService : ICrudModelService<Guid, UserDto, UserGridDto>
{
    /// <summary>
    /// Gets items filtered.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<UserDto>> GetFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default);
}