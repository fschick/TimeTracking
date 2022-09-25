using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Filter;
using Ical.Net;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IHolidayService" />
public class HolidayService : CrudModelService<Holiday, HolidayDto, HolidayGridDto>, IHolidayService
{
    /// <inheritdoc />
    public HolidayService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<HolidayGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateHolidayFilter(filters);

        return await Repository
            .Get<Holiday, HolidayGridDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.StartDateLocal)
                    .ThenBy(x => x.EndDateLocal)
                    .ThenBy(x => x.Title),
                cancellationToken: cancellationToken
            );
    }

    /// <inheritdoc />
    public async Task Import(IFormFile file, HolidayType type, CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream();
        await using var inputStream = file.OpenReadStream();
        await inputStream.CopyToAsync(memoryStream, cancellationToken);
        var calendarData = Encoding.UTF8.GetString(memoryStream.ToArray());

        var holidays = Calendar.Load(calendarData)
            .Events
            .Select(holiday => new HolidayDto
            {
                Title = holiday.Summary,
                StartDate = holiday.Start.AsDateTimeOffset,
                EndDate = holiday.IsAllDay
                    ? holiday.End.AsDateTimeOffset.AddTicks(-1).Date
                    : holiday.End.AsDateTimeOffset.Date,
                Type = type
            })
            .ToList();

        var entities = Mapper.Map<List<Holiday>>(holidays);
        // ReSharper disable once MethodSupportsCancellation
        await Repository.AddRange(entities);
        // ReSharper disable once MethodSupportsCancellation
        await Repository.SaveChanges();
    }
}
