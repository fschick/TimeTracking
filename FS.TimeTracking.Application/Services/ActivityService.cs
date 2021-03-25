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
    /// <inheritdoc cref="IActivityService" />
    public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityListDto>, IActivityService
    {
        /// <inheritdoc />
        public ActivityService(IRepository repository, IModelConverter<Activity, ActivityDto> modelConverter)
            : base(repository, modelConverter)
        {
        }

        /// <inheritdoc />
        public override Task<List<ActivityListDto>> List(Guid? id, CancellationToken cancellationToken = default)
            => Repository
                .Get(
                    select: (Activity activity) => new ActivityListDto
                    {
                        Id = activity.Id,
                        Name = activity.Name,
                        CustomerShortName = activity.Customer.ShortName,
                        CustomerCompanyName = activity.Customer.CompanyName,
                        ProjectName = activity.Project.Name,
                        Hidden = activity.Hidden,
                    },
                    where: id.HasValue ? x => x.Id == id : null,
                    cancellationToken: cancellationToken
                );
    }
}
