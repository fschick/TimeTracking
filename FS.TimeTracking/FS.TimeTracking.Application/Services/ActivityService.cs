using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="IActivityService" />
    public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityListDto>, IActivityService
    {
        /// <inheritdoc />
        public ActivityService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }

        /// <inheritdoc />
        public override async Task<List<ActivityListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
            => await ListInternal(id, o => o.OrderBy(x => x.Title), cancellationToken);
    }
}
