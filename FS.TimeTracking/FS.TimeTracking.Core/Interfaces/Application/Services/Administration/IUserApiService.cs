using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using System;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Administration;

/// <inheritdoc />
public interface IUserApiService : ICrudModelService<Guid, UserDto, UserGridDto>
{
}