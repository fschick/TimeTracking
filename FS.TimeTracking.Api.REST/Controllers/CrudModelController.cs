using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Models.REST;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <inheritdoc cref="ICrudModelService{TDto, TQueryDto}" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="ICrudModelService{TDto, TQueryDto}" />
    public abstract class CrudModelController<TDto, TQueryDto> : ControllerBase, ICrudModelService<TDto, TQueryDto>
    {
        private readonly ICrudModelService<TDto, TQueryDto> _modelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrudModelController{TDto, TQueryDto}"/> class.
        /// </summary>
        /// <param name="modelService">The model service.</param>
        protected CrudModelController(ICrudModelService<TDto, TQueryDto> modelService)
            => _modelService = modelService;

        /// <inheritdoc />
        [HttpGet]
        public Task<List<TQueryDto>> Query(CancellationToken cancellationToken = default)
            => _modelService.Query(cancellationToken);

        /// <inheritdoc />
        [HttpGet("{id}", Name = "[controller]_[action]")]
        public Task<TDto> Get(Guid id, CancellationToken cancellationToken = default)
            => _modelService.Get(id, cancellationToken);

        /// <inheritdoc />
        [HttpPost]
        public Task<TDto> Create(TDto dto)
            => _modelService.Create(dto);

        /// <inheritdoc />
        [HttpPut]
        public Task<TDto> Update(TDto dto)
            => _modelService.Update(dto);

        /// <inheritdoc />
        [HttpDelete("{id}", Name = "[controller]_[action]")]
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorInformation), (int)HttpStatusCode.Conflict)]
        public Task<long> Delete(Guid id)
            => _modelService.Delete(id);
    }
}
