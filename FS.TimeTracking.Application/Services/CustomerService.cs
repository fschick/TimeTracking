using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="ICustomerService" />
    public class CustomerService : CrudModelService<Customer, CustomerDto, CustomerDto>, ICustomerService
    {
        /// <inheritdoc />
        public CustomerService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }
    }
}
