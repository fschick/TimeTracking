﻿using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IProjectService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderService" />
[V1ApiController]
public class OrderController : CrudModelController<OrderDto, OrderListDto>, IOrderService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public OrderController(IOrderService modelService)
        : base(modelService)
    {
    }
}