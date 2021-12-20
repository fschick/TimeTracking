using AutoMapper;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.Extensions;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IProjectService" />
public class OrderService : CrudModelService<Order, OrderDto, OrderListDto>, IOrderService
{
    /// <inheritdoc />
    public OrderService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<OrderListDto>> GetListFiltered(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateOrderFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);

        return await Repository
            .Get<Order, OrderListDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.Hidden)
                    .ThenByDescending(x => x.StartDateLocal)
                    .ThenBy(x => x.Customer.Title)
                    .ThenBy(x => x.Title),
                cancellationToken: cancellationToken
            );
    }
}