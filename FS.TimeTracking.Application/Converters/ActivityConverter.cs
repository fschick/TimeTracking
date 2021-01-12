﻿using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Converters
{
    public class ActivityConverter : IModelConverter<Activity, ActivityDto>
    {
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
