using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Converters
{
    public class ProjectConverter : IModelConverter<Project, ProjectDto>
    {
        public ProjectDto ToDto(Project model)
            => new ProjectDto
            {
                Id = model.Id,
                Name = model.Name,
                CustomerId = model.CustomerId,
                Comment = model.Comment,
                Hidden = model.Hidden
            };

        public Project FromDto(ProjectDto dto)
            => new Project
            {
                Id = dto.Id,
                Name = dto.Name,
                CustomerId = dto.CustomerId,
                Comment = dto.Comment,
                Hidden = dto.Hidden
            };
    }
}
