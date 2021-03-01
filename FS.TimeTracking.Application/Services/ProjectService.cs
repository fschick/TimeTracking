using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="IProjectService" />
    public class ProjectService : CrudModelService<Project, ProjectDto, ProjectDto>, IProjectService
    {
        /// <inheritdoc />
        public ProjectService(IRepository repository, IModelConverter<Project, ProjectDto> modelConverter)
            : base(repository, modelConverter)
        {
        }

        /// <inheritdoc />
        public override Task<List<ProjectDto>> List(Guid? id, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}
