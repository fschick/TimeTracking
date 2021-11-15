using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="ICustomerService" />
public class CustomerService : CrudModelService<Customer, CustomerDto, CustomerListDto>, ICustomerService
{
    /// <inheritdoc />
    public CustomerService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<CustomerListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
        => await ListInternal(
            id,
            o => o
                .OrderBy(x => x.Hidden)
                .ThenBy(x => x.Title)
                .ThenBy(x => x.CompanyName)
                .ThenBy(x => x.ContactName),
            cancellationToken
        );
}