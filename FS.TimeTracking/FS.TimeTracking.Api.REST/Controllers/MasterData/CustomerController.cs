using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="ICustomerService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICustomerService" />
[V1ApiController]
public class CustomerController : CrudModelController<CustomerDto, CustomerGridDto>, ICustomerService
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