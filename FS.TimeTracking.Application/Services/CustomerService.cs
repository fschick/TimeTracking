using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    public class CustomerService : CrudModelService<Customer, CustomerDto>, ICustomerService
    {
        public CustomerService(IRepository repository, IModelConverter<Customer, CustomerDto> modelConverter)
            : base(repository, modelConverter)
        {
        }
    }
}
