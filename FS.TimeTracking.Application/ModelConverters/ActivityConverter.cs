using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.ModelConverters
{
    /// <inheritdoc />
    public class ActivityConverter : IModelConverter<Activity, ActivityDto>
    {
        /// <inheritdoc />
        public ActivityDto ToDto(Activity model)
            => new ActivityDto
            {
                Id = model.Id,
                Name = model.Name,
                CustomerId = model.CustomerId,
                ProjectId = model.ProjectId,
                Comment = model.Comment,
                Hidden = model.Hidden
            };

        /// <inheritdoc />
        public Activity FromDto(ActivityDto dto)
            => new Activity
            {
                Id = dto.Id,
                Name = dto.Name,
                CustomerId = dto.CustomerId,
                ProjectId = dto.ProjectId,
                Comment = dto.Comment,
                Hidden = dto.Hidden
            };
    }
}
