﻿using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;

/// <inheritdoc />
public interface IOrderService : ICrudModelService<OrderDto, OrderListDto>
{
}