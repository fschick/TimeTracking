using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <inheritdoc cref="ICrudModelService{TDto}" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="ICrudModelService{TDto}" />
    public abstract class CrudModelController<TDto> : ControllerBase, ICrudModelService<TDto>
    {
        private readonly ICrudModelService<TDto> _modelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrudModelController{TDto}"/> class.
        /// </summary>
        /// <param name="modelService">The model service.</param>
        protected CrudModelController(ICrudModelService<TDto> modelService)
            => _modelService = modelService;

        /// <inheritdoc />
        [HttpGet]
        public Task<List<TDto>> Query(CancellationToken cancellationToken = default)
            => _modelService.Query(cancellationToken);

        /// <inheritdoc />
        [HttpGet("{id}")]
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
        [HttpDelete]
        public Task<long> Delete(Guid id)
            => _modelService.Delete(id);
    }
}
