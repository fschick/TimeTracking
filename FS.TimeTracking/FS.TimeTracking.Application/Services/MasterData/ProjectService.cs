using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Filter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="IProjectApiService" />
public class ProjectService : CrudModelService<Project, ProjectDto, ProjectGridDto>, IProjectApiService
{
    /// <inheritdoc />
    public ProjectService(IAuthorizationService authorizationService, IDbRepository dbRepository, IMapper mapper, IFilterFactory filterFactory, IUserService userService)
        : base(authorizationService, dbRepository, mapper, filterFactory, userService)
    { }

    /// <inheritdoc />
    public override async Task<List<ProjectGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = await FilterFactory.CreateProjectFilter(filters);

        var gridItems = await DbRepository
            .Get<Project, ProjectGridDto>(
                where: filter,
                orderBy: o => o
                    .OrderBy(x => x.Hidden)
                    .ThenBy(x => x.Title)
                    .ThenBy(x => x.Customer.Title),
                cancellationToken: cancellationToken
            );

        await AuthorizationService.SetAuthorizationRelatedProperties(gridItems, cancellationToken);

        return gridItems;
    }
}