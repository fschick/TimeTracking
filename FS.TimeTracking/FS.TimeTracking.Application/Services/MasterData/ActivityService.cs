using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IActivityService" />
public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityListDto>, IActivityService
{
    /// <inheritdoc />
    public ActivityService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<ActivityListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
        => await ListInternal(
            id,
            o => o
                .OrderBy(x => x.Hidden)
                .ThenBy(x => x.Title)
                .ThenBy(x => x.Project.Customer.Title)
                .ThenBy(x => x.Project.Title),
            cancellationToken
        );
}