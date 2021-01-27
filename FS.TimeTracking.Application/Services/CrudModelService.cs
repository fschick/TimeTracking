using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Models;
using FS.TimeTracking.Shared.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    public abstract class CrudModelService<TModel, TDto> : ICrudModelService<TDto>
        where TModel : class, IEntityModel, new()
    {
        private readonly IRepository _repository;
        private readonly IModelConverter<TModel, TDto> _modelConverter;

        protected CrudModelService(IRepository repository, IModelConverter<TModel, TDto> modelConverter)
        {
            _repository = repository;
            _modelConverter = modelConverter;
        }

        public async Task<List<TDto>> Query(CancellationToken cancellationToken = default)
            => await _repository
                .Get(
                    select: (TModel x) => _modelConverter.ToDto(x),
                    cancellationToken: cancellationToken
                );

        public async Task<TDto> Get(Guid id, CancellationToken cancellationToken = default)
            => await _repository
                .FirstOrDefault(
                    select: (TModel x) => _modelConverter.ToDto(x),
                    where: x => x.Id == id,
                    cancellationToken: cancellationToken
                );

        public async Task<TDto> Create(TDto dto)
        {
            var result = await _repository.Add(_modelConverter.FromDto(dto));
            await _repository.SaveChanges();
            return _modelConverter.ToDto(result);
        }

        public async Task<TDto> Update(TDto dto)
        {
            var result = _repository.Update(_modelConverter.FromDto(dto));
            await _repository.SaveChanges();
            return _modelConverter.ToDto(result);
        }

        public async Task<long> Delete(Guid id)
        {
            await _repository.Remove<TModel>(x => x.Id == id);
            return await _repository.SaveChanges();
        }
    }
}