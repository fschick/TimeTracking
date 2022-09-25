using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Filter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IActivityService" />
public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityGridDto>, IActivityService
{
    /// <inheritdoc />
    public ActivityService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<ActivityGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateActivityFilter(filters);

        return await Repository
            .Get<Activity, ActivityGridDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.Hidden)
                    .ThenBy(x => x.Title)
                    .ThenBy(x => x.Project.Customer.Title)
                    .ThenBy(x => x.Project.Title),
                cancellationToken: cancellationToken
            );
    }
}