using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IActivityService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IActivityService" />
[V1ApiController]
public class ActivityController : CrudModelController<ActivityDto, ActivityListDto>, IActivityService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public ActivityController(IActivityService modelService)
        : base(modelService)
    {
    }
}