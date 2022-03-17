using AutoMapper;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Extensions;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Abstractions.Interfaces.Repository.Services;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IActivityService" />
public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityListDto>, IActivityService
{
    /// <inheritdoc />
    public ActivityService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<ActivityListDto>> GetListFiltered(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateActivityFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);

        return await Repository
            .Get<Activity, ActivityListDto>(
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