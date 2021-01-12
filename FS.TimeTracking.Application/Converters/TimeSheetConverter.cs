using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Converters
{
    public class TimeSheetConverter : IModelConverter<TimeSheet, TimeSheetDto>
    {
        public TimeSheetDto ToDto(TimeSheet model)
            => new TimeSheetDto
            {
                Id = model.Id,
                CustomerId = model.CustomerId,
                ActivityId = model.ActivityId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Billable = model.Billable,
                Comment = model.Comment
            };

        public TimeSheet FromDto(TimeSheetDto dto)
            => new TimeSheet
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                ActivityId = dto.ActivityId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Billable = dto.Billable,
                Comment = dto.Comment
            };
    }
}
