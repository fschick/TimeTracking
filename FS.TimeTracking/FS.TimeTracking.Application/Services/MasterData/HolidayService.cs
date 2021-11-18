using AutoMapper;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IHolidayService" />
public class HolidayService : CrudModelService<Holiday, HolidayDto, HolidayListDto>, IHolidayService
{
    /// <inheritdoc />
    public HolidayService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<HolidayListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
        => await ListInternal(
            id,
            o => o
                .OrderBy(x => x.StartDateLocal)
                .ThenBy(x => x.EndDateLocal)
                .ThenBy(x => x.Title),
            cancellationToken
        );
}