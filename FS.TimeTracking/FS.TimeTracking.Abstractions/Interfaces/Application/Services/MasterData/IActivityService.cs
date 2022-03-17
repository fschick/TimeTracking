using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.MasterData;

/// <inheritdoc />
public interface IActivityService : ICrudModelService<ActivityDto, ActivityListDto>
{
}