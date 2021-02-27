using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <inheritdoc cref="IProjectService" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="IProjectService" />
    [V1ApiController]
    public class ProjectController : CrudModelController<ProjectDto, ProjectDto>, IProjectService
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
}
