using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.REST;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <inheritdoc cref="ICrudModelService{TDto,TGridDto}" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICrudModelService{TDto, TGridDto}" />
public abstract class CrudModelController<TDto, TGridDto> : ControllerBase, ICrudModelService<TDto, TGridDto>
{
    private readonly ICrudModelService<TDto, TGridDto> _modelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudModelController{TDto, TGridDto}"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    protected CrudModelController(ICrudModelService<TDto, TGridDto> modelService)
        => _modelService = modelService;

    /// <inheritdoc />
    [NotFoundWhenEmpty]
    [HttpGet("{id:guid}", Name = "[controller]_[action]")]
    public async Task<TDto> Get(Guid id, CancellationToken cancellationToken = default)
        => await _modelService.Get(id, cancellationToken);

    /// <inheritdoc />
    [NotFoundWhenEmpty]
    [HttpGet]
    public async Task<List<TGridDto>> GetGridFiltered([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _modelService.GetGridFiltered(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);

    /// <inheritdoc />
    [NotFoundWhenEmpty]
    [HttpGet("{id:guid}", Name = "[controller]_[action]")]
    public async Task<TGridDto> GetGridItem(Guid id, CancellationToken cancellationToken = default)
        => await _modelService.GetGridItem(id, cancellationToken);

    /// <inheritdoc />
    [HttpPost]
    public async Task<TDto> Create(TDto dto)
        => await _modelService.Create(dto);

    /// <inheritdoc />
    [HttpPut]
    public async Task<TDto> Update(TDto dto)
        => await _modelService.Update(dto);

    /// <inheritdoc />
    [HttpDelete("{id:guid}", Name = "[controller]_[action]")]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorInformation), (int)HttpStatusCode.Conflict)]
    public async Task<long> Delete(Guid id)
        => await _modelService.Delete(id);
}