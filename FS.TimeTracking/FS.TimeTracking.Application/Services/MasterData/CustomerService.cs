using AutoMapper;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.Application.MasterData;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="ICustomerService" />
public class CustomerService : CrudModelService<Customer, CustomerDto, CustomerListDto>, ICustomerService
{
    /// <inheritdoc />
    public CustomerService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<CustomerListDto>> GetListFiltered(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateCustomerFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);

        return await Repository
            .Get<Customer, CustomerListDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.Hidden)
                    .ThenBy(x => x.Title)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactName),
                cancellationToken: cancellationToken
            );
    }
}