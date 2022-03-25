using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;

/// <summary>
/// CRUD model services
/// </summary>
/// <typeparam name="TDto">The type of the entity DTO.</typeparam>
/// <typeparam name="TGridDto">The type of the DTO used to deliver a flatten view to the entity</typeparam>
public interface ICrudModelService<TDto, TGridDto>
{
    /// <summary>
    /// Gets the item specified by <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<TDto> Get(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets items as filtered grid rows.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<TGridDto>> GetGridFiltered(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single grid row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<TGridDto> GetGridItem(Guid id, CancellationToken cancellationToken = default);

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