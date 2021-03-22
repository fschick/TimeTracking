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
    public class ProjectService : CrudModelService<Project, ProjectDto, ProjectListDto>, IProjectService
    {
        /// <inheritdoc />
        public ProjectService(IRepository repository, IModelConverter<Project, ProjectDto> modelConverter)
            : base(repository, modelConverter)
        {
        }

        /// <inheritdoc />
        public override Task<List<ProjectListDto>> List(Guid? id, CancellationToken cancellationToken = default)
            => Repository
                .Get(
                    select: (Project project) => new ProjectListDto
                    {
                        Id = project.Id,
                        Name = project.Name,
                        CustomerShortName = project.Customer.ShortName,
                        CustomerCompanyName = project.Customer.CompanyName,
                        Hidden = project.Hidden,
                    },
                    where: id.HasValue ? x => x.Id == id : null,
                    cancellationToken: cancellationToken
                );
    }
}
