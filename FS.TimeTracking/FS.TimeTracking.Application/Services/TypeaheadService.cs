﻿using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc />
    public class TypeaheadService : ITypeaheadService
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeaheadService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <autogeneratedoc />
        public TypeaheadService(IRepository repository)
            => _repository = repository;

        /// <inheritdoc />
        public async Task<List<TypeaheadDto<string>>> GetCustomers(CancellationToken cancellationToken = default)
            => await _repository
                .Get(
                    select: (Customer x) => TypeaheadDto.Create(x.Id, x.Title),
                    where: x => !x.Hidden,
                    orderBy: o => o.OrderBy(x => x.Title),
                    cancellationToken: cancellationToken
                );

        /// <inheritdoc />
        public async Task<List<TypeaheadDto<string>>> GetProjects(CancellationToken cancellationToken = default)
            => await _repository
                .Get(
                    select: (Project x) => TypeaheadDto.Create(x.Id, $"{x.Title} ({x.Customer.Title})"),
                    where: x => !x.Hidden,
                    orderBy: o => o.OrderBy(x => x.Title).ThenBy(x => x.Customer.Title),
                    cancellationToken: cancellationToken
                );

        /// <inheritdoc />
        public async Task<List<TypeaheadDto<string>>> GetOrders(CancellationToken cancellationToken = default)
            => await _repository
                .Get(
                    select: (Order x) => TypeaheadDto.Create(x.Id, x.Title),
                    where: x => !x.Hidden,
                    orderBy: o => o.OrderBy(x => x.Title),
                    cancellationToken: cancellationToken
                );

        /// <inheritdoc />
        public async Task<List<TypeaheadDto<string>>> GetOrderNumbers(CancellationToken cancellationToken = default)
            => await _repository
                .Get(
                    select: (Order x) => TypeaheadDto.Create(x.Id, x.Number),
                    where: x => !x.Hidden,
                    orderBy: o => o.OrderBy(x => x.Number),
                    cancellationToken: cancellationToken
                );

        /// <inheritdoc />
        public async Task<List<TypeaheadDto<string>>> GetActivities(CancellationToken cancellationToken = default)
            => await _repository
                .Get(
                    select: (Activity x) => TypeaheadDto.Create(x.Id, x.Title),
                    where: x => !x.Hidden,
                    orderBy: o => o.OrderBy(x => x.Title),
                    cancellationToken: cancellationToken
                );
    }
}
