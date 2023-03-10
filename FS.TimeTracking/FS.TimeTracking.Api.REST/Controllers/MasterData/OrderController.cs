using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IProjectApiService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderApiService" />
[ApiV1Controller]
[Authorize(Policy = PermissionName.MASTER_DATA_ORDERS)]
[ExcludeFromCodeCoverage]
public class OrderController : CrudModelController<OrderDto, OrderGridDto>, IOrderApiService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public OrderController(IOrderApiService modelService)
        : base(modelService)
    {
    }
}