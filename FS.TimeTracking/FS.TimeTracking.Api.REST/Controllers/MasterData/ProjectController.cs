using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IProjectService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IProjectService" />
[ApiV1Controller]
[Authorize(Policy = PermissionNames.MASTER_DATA_PROJECTS)]
[ExcludeFromCodeCoverage]
public class ProjectController : CrudModelController<ProjectDto, ProjectGridDto>, IProjectService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public ProjectController(IProjectService modelService)
        : base(modelService)
    {
    }
}