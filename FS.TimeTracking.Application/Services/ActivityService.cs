using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="IActivityService" />
    public class ActivityService : CrudModelService<Activity, ActivityDto>, IActivityService
    {
        /// <inheritdoc />
        public ActivityService(IRepository repository, IModelConverter<Activity, ActivityDto> modelConverter)
            : base(repository, modelConverter)
        {
        }
    }
}
