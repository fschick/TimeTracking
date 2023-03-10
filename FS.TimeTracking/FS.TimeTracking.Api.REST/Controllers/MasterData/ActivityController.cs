using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IActivityApiService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IActivityApiService" />
[ApiV1Controller]
[Authorize(Policy = PermissionName.MASTER_DATA_ACTIVITIES)]
[ExcludeFromCodeCoverage]
public class ActivityController : CrudModelController<ActivityDto, ActivityGridDto>, IActivityApiService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public ActivityController(IActivityApiService modelService)
        : base(modelService)
    {
    }
}