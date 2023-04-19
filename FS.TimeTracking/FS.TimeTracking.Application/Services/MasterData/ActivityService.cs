using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.Core;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IActivityApiService" />
public class ActivityService : CrudModelService<Activity, ActivityDto, ActivityGridDto>, IActivityApiService
{
    /// <inheritdoc/>
    public ActivityService(IAuthorizationService authorizationService, IDbRepository dbRepository, IMapper mapper, IFilterFactory filterFactory, IUserService userService)
        : base(authorizationService, dbRepository, mapper, filterFactory, userService)
    { }

    /// <inheritdoc />
    public override async Task<List<ActivityGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = await FilterFactory.CreateActivityFilter(filters);

        var gridItems = await DbRepository
            .Get<Activity, ActivityGridDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.Hidden)
                    .ThenBy(x => x.Title)
                    .ThenBy(x => x.Customer.Title)
                    .ThenBy(x => x.Project.Title),
                cancellationToken: cancellationToken
            );

        await AuthorizationService.SetAuthorizationRelatedProperties(gridItems, cancellationToken);

        return gridItems;
    }

    /// <inheritdoc />
    protected override async Task CheckConformity(Activity model)
    {
        await base.CheckConformity(model);
        await EnsureCustomerOfActivityAndProjectMatches(model);
        await EnsureNoTimeSheetWithDifferentProjectAssigned(model);
        await EnsureNoTimeSheetWithDifferentCustomerAssigned(model);
    }

    private async Task EnsureCustomerOfActivityAndProjectMatches(Activity model)
    {
        if (model.CustomerId == null || model.ProjectId == null)
            return;

        var projectCustomerId = await DbRepository
            .FirstOrDefault(
                select: (Project x) => x.CustomerId,
                where: x => x.Id == model.ProjectId
            );

        if (projectCustomerId == null)
            return;

        if (model.CustomerId != projectCustomerId)
            throw new ConformityException("Customer of activity does not match customer of related project.");
    }

    private async Task EnsureNoTimeSheetWithDifferentProjectAssigned(Activity model)
    {
        var isNewActivity = model.Id == Guid.Empty;
        if (isNewActivity || model.ProjectId == null)
            return;

        var timeSheetProjectIds = await DbRepository
            .GetGrouped(
                (TimeSheet timeSheet) => timeSheet.ProjectId,
                projectGroup => projectGroup.Key,
                timeSheet => timeSheet.ActivityId == model.Id
            );

        if (!timeSheetProjectIds.Any())
            return;

        var differentProjectAssigned = timeSheetProjectIds[0] != model.ProjectId;
        var assignedToMultipleProjects = timeSheetProjectIds.Count > 1;
        if (differentProjectAssigned || assignedToMultipleProjects)
            throw new ConflictException(
                ApplicationErrorCode.ConflictActivityAlreadyAssignedToDifferentProjects,
                "Activity is already assigned to different projects via time sheets."
            );
    }

    private async Task EnsureNoTimeSheetWithDifferentCustomerAssigned(Activity model)
    {
        var isNewActivity = model.Id == Guid.Empty;
        if (isNewActivity || model.CustomerId == null)
            return;

        var timeSheetCustomerIds = await DbRepository
            .GetGrouped(
                (TimeSheet timeSheet) => timeSheet.CustomerId,
                customerGroup => customerGroup.Key,
                timeSheet => timeSheet.ActivityId == model.Id
            );

        if (!timeSheetCustomerIds.Any())
            return;

        var differentCustomerAssigned = timeSheetCustomerIds[0] != model.CustomerId;
        var assignedToMultipleCustomers = timeSheetCustomerIds.Count > 1;
        if (differentCustomerAssigned || assignedToMultipleCustomers)
            throw new ConflictException(
                ApplicationErrorCode.ConflictActivityAlreadyAssignedToDifferentCustomers,
                "Activity is already assigned to different customers via time sheets."
            );
    }
}