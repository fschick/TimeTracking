using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Models;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <inheritdoc cref="ICrudModelService{TKey, TDto,TGridDto}" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICrudModelService{TKey, TDto, TGridDto}" />
[ExcludeFromCodeCoverage]
public abstract class CrudModelController<TKey, TDto, TGridDto> : ControllerBase, ICrudModelService<TKey, TDto, TGridDto>
{
    private readonly ICrudModelService<TKey, TDto, TGridDto> _modelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudModelController{TKey, TDto, TGridDto}"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    protected CrudModelController(ICrudModelService<TKey, TDto, TGridDto> modelService)
        => _modelService = modelService;

    /// <inheritdoc />
    [NotFoundWhenEmpty]
    [HttpGet("{id:guid}", Name = "[controller]_[action]")]
    public async Task<TDto> Get(TKey id, CancellationToken cancellationToken = default)
        => await _modelService.Get(id, cancellationToken);

    /// <inheritdoc />
    [NotFoundWhenEmpty]
    [HttpGet]
    public async Task<List<TGridDto>> GetGridFiltered([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _modelService.GetGridFiltered(filters, cancellationToken);

    /// <inheritdoc />
    [NotFoundWhenEmpty]
    [HttpGet("{id:guid}", Name = "[controller]_[action]")]
    public async Task<TGridDto> GetGridItem(TKey id, CancellationToken cancellationToken = default)
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
    public async Task<long> Delete(TKey id)
        => await _modelService.Delete(id);
}