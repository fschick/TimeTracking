using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Models;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <inheritdoc cref="ICrudModelService{TKey, TDto,TGridDto}" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICrudModelService{TKey, TDto, TGridDto}" />
[Authorize]
[ExcludeFromCodeCoverage]
public abstract class CrudModelController<TDto, TGridDto> : ControllerBase, ICrudModelService<Guid, TDto, TGridDto>
{
    private readonly ICrudModelService<Guid, TDto, TGridDto> _modelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudModelController{TDto, TGridDto}"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    protected CrudModelController(ICrudModelService<Guid, TDto, TGridDto> modelService)
        => _modelService = modelService;

    /// <inheritdoc />
    [HttpGet("{id:guid}", Name = "[controller]_[action]")]
    [NotFoundWhenEmpty]
    public async Task<TDto> Get(Guid id, CancellationToken cancellationToken = default)
        => await _modelService.Get(id, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [NotFoundWhenEmpty]
    public async Task<List<TGridDto>> GetGridFiltered([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _modelService.GetGridFiltered(filters, cancellationToken);

    /// <inheritdoc />
    [HttpGet("{id:guid}", Name = "[controller]_[action]")]
    [NotFoundWhenEmpty]
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
    [ProducesResponseType(typeof(ApplicationError), (int)HttpStatusCode.Conflict)]
    public async Task<long> Delete(Guid id)
        => await _modelService.Delete(id);
}