using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="IActivityService" />
    public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityListDto>, IActivityService
    {
        /// <inheritdoc />
        public ActivityService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }
    }
}
