using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
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
        public override async Task<List<CustomerDto>> List(Guid? id, CancellationToken cancellationToken = default)
            => await Repository
                .Get(
                    select: (Customer customer) => ModelConverter.ToDto(customer),
                    where: id.HasValue ? x => x.Id == id : null,
                    cancellationToken: cancellationToken
                );
    }
}
