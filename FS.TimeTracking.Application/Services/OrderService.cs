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
    /// <inheritdoc cref="IProjectService" />
    public class OrderService : CrudModelService<Order, OrderDto, OrderListDto>, IOrderService
    {
        /// <inheritdoc />
        public OrderService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }

        /// <inheritdoc />
        public override async Task<List<OrderListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
            => await ListInternal(id, o => o.OrderBy(x => x.StartDate), cancellationToken);
    }
}
