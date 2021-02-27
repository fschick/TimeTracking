﻿using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="ICustomerService" />
    public class CustomerService : CrudModelService<Customer, CustomerDto, CustomerDto>, ICustomerService
    {
        /// <inheritdoc />
        public CustomerService(IRepository repository, IModelConverter<Customer, CustomerDto> modelConverter)
            : base(repository, modelConverter)
        {
        }

        /// <inheritdoc />
        public override async Task<List<CustomerDto>> List(CancellationToken cancellationToken = default)
            => await Repository
                .Get(
                    select: (Customer x) => ModelConverter.ToDto(x),
                    cancellationToken: cancellationToken
                );
    }
}
