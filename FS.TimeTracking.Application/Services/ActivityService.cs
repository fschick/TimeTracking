using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    public class ActivityService : CrudModelService<Activity, ActivityDto>, IActivityService
    {
        public ActivityService(IRepository repository, IModelConverter<Activity, ActivityDto> modelConverter)
            : base(repository, modelConverter)
        {
        }
    }
}
