using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="ICustomerService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICustomerService" />
[ApiV1Controller]
[ExcludeFromCodeCoverage]
public class CustomerController : CrudModelController<Guid, CustomerDto, CustomerGridDto>, ICustomerService
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