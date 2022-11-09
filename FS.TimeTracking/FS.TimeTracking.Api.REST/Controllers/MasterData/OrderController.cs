﻿using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IProjectService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderService" />
[V1ApiController]
[ExcludeFromCodeCoverage]
public class OrderController : CrudModelController<Guid, OrderDto, OrderGridDto>, IOrderService
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