using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="ICustomerApiService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICustomerApiService" />
[ApiV1Controller]
[Authorize(Policy = PermissionName.MASTER_DATA_CUSTOMERS)]
[ExcludeFromCodeCoverage]
public class CustomerController : CrudModelController<CustomerDto, CustomerGridDto>, ICustomerApiService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public CustomerController(ICustomerApiService modelService)
        : base(modelService)
    {
    }
}