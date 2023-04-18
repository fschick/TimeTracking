﻿using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.Core;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Filter;
using Ical.Net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IHolidayApiService" />
public class HolidayService : CrudModelService<Holiday, HolidayDto, HolidayGridDto>, IHolidayApiService
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HolidayService"/> class.
    /// </summary>
    /// <param name="dbRepository">The database repository.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="filterFactory">The filter factory.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="authorizationService">The authorization service.</param>
    /// <autogeneratedoc />
    public HolidayService(IAuthorizationService authorizationService, IDbRepository dbRepository, IMapper mapper, IFilterFactory filterFactory, IUserService userService)
        : base(authorizationService, dbRepository, mapper, filterFactory)
        => _userService = userService;

    /// <inheritdoc />
    public override async Task<HolidayDto> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await base.Get(id, cancellationToken);
        if (result == null)
            return null;

        if (!AuthorizationService.CanViewUser(result.UserId))
            throw new ForbiddenException(ApplicationErrorCode.ForbiddenForeignUserData);

        return result;
    }

    /// <inheritdoc />
    public override async Task<List<HolidayGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = await FilterFactory.CreateHolidayFilter(filters);

        var gridItems = await DbRepository
            .Get<Holiday, HolidayGridDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.StartDateLocal)
                    .ThenBy(x => x.EndDateLocal)
                    .ThenBy(x => x.Title),
                cancellationToken: cancellationToken
        );

        await _userService.SetUserRelatedProperties(filters, gridItems, cancellationToken);
        await AuthorizationService.SetAuthorizationRelatedProperties(gridItems, cancellationToken);

        return gridItems;
    }

    /// <inheritdoc />
    public override async Task<HolidayGridDto> GetGridItem(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await base.GetGridItem(id, cancellationToken);
        if (result == null)
            return null;

        if (!AuthorizationService.CanViewUser(result.UserId))
            throw new ForbiddenException(ApplicationErrorCode.ForbiddenForeignUserData);

        await _userService.SetUserRelatedProperties(result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public override async Task<HolidayDto> Create(HolidayDto dto)
    {
        if (!AuthorizationService.CanManageUser(dto.UserId))
            throw new ForbiddenException(ApplicationErrorCode.ForbiddenForeignUserData);

        return await base.Create(dto);
    }

    /// <inheritdoc />
    public override async Task<HolidayDto> Update(HolidayDto dto)
    {
        if (!await AuthorizationService.CanManageUser<HolidayDto, Holiday>(dto))
            throw new ForbiddenException(ApplicationErrorCode.ForbiddenForeignUserData);

        return await base.Update(dto);
    }

    /// <inheritdoc />
    public override async Task<long> Delete(Guid id)
    {
        if (!await AuthorizationService.CanManageUser<Holiday>(id))
            throw new ForbiddenException(ApplicationErrorCode.ForbiddenForeignUserData);

        return await base.Delete(id);
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
        await DbRepository.AddRange(entities);
        // ReSharper disable once MethodSupportsCancellation
        await DbRepository.SaveChanges();
    }
}
