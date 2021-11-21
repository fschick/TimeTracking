using AutoMapper;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Enums;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using Ical.Net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.DTOs.MasterData;

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
