using AutoMapper;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.TimeTracking;

/// <inheritdoc cref="ITimeSheetService" />
public class TimeSheetService : CrudModelService<TimeSheet, TimeSheetDto, TimeSheetListDto>, ITimeSheetService
{
    /// <inheritdoc />
    public TimeSheetService(IRepository repository, IMapper mapper)
        : base(repository, mapper)
    { }

    /// <inheritdoc />
    public override async Task<List<TimeSheetListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
        => await Repository
            .Get<TimeSheet, TimeSheetListDto>(
                where: id.HasValue ? x => x.Id == id : null,
                orderBy: o => o.OrderByDescending(x => x.StartDateLocal),
                cancellationToken: cancellationToken
            );

    /// <inheritdoc />
    public async Task<List<TimeSheetListDto>> ListFiltered(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, CancellationToken cancellationToken = default)
    {
        var projectCustomerFilter = projectFilter.Cast<Project>()
            .Replace(x => x.Customer, customerFilter.Cast<Customer>());

        var filter = timeSheetFilter
            .Cast<TimeSheet>()
            .Replace(x => x.Project, projectCustomerFilter)
            .Replace(x => x.Activity, activityFilter.Cast<Activity>())
            .Replace(x => x.Order, orderFilter.Cast<Order>());

        return await Repository
            .Get<TimeSheet, TimeSheetListDto>(
                where: filter,
                orderBy: o => o.OrderByDescending(x => x.StartDateLocal),
                cancellationToken: cancellationToken
            );
    }

    /// <inheritdoc />
    public async Task<TimeSheetDto> StartSimilarTimeSheetEntry(Guid copyFromTimesheetId, DateTimeOffset startDateTime)
    {
        var timeSheet = await Repository.FirstOrDefault((TimeSheet x) => x, x => x.Id == copyFromTimesheetId);
        if (timeSheet == null)
            throw new InvalidOperationException($"Time sheet with ID {copyFromTimesheetId} does not exists.");

        var timeSheetCopy = Mapper.Map<TimeSheetDto>(timeSheet);
        timeSheetCopy.Id = Guid.NewGuid();
        timeSheetCopy.StartDate = startDateTime;
        timeSheetCopy.EndDate = null;
        return await Create(timeSheetCopy);
    }

    /// <inheritdoc />
    public async Task<TimeSheetDto> StopTimeSheetEntry(Guid timesheetId, DateTimeOffset endDateTime)
    {
        var timeSheet = await Repository.FirstOrDefault((TimeSheet x) => x, x => x.Id == timesheetId);
        if (timeSheet.EndDate != null)
            throw new InvalidOperationException($"Time sheet with ID {timesheetId} is already stopped.");

        timeSheet.EndDate = endDateTime;
        timeSheet = Repository.Update(timeSheet);
        await Repository.SaveChanges();
        return Mapper.Map<TimeSheetDto>(timeSheet);
    }
}