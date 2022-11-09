﻿using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using System;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;

/// <inheritdoc />
public interface IActivityService : ICrudModelService<Guid, ActivityDto, ActivityGridDto>
{
}