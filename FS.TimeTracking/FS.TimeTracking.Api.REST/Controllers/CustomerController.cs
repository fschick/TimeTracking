using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers;

/// <inheritdoc cref="ICustomerService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICustomerService" />
[V1ApiController]
public class CustomerController : CrudModelController<CustomerDto, CustomerListDto>, ICustomerService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public CustomerController(ICustomerService modelService)
        : base(modelService)
    {
    }
}