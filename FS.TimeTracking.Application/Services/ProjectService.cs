using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    public class ProjectService : CrudModelService<Project, ProjectDto>, IProjectService
    {
        public ProjectService(IRepository repository, IModelConverter<Project, ProjectDto> modelConverter)
            : base(repository, modelConverter)
        {
        }
    }
}
