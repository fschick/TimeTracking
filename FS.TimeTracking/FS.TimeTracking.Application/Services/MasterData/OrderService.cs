using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Filter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IProjectService" />
public class OrderService : CrudModelService<Order, OrderDto, OrderGridDto>, IOrderService
{
    /// <inheritdoc />
    public OrderService(IDbRepository dbRepository, IMapper mapper)
        : base(dbRepository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<OrderGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateOrderFilter(filters);

        return await DbRepository
            .Get<Order, OrderGridDto>(
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