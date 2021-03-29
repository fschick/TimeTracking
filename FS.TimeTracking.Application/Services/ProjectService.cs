using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="IProjectService" />
    public class ProjectService : CrudModelService<Project, ProjectDto, ProjectListDto>, IProjectService
    {
        /// <inheritdoc />
        public ProjectService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }
    }
}
