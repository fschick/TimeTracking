﻿using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.Extensions;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.Application.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Report;

/// <inheritdoc />
public class ProjectReportService : IProjectReportService
{
    private readonly ISettingService _settingService;
    private readonly IRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectReportService" /> class.
    /// </summary>
    /// <param name="settingService">The setting service.</param>
    /// <param name="repository">The repository.</param>
    /// <autogeneratedoc />
    public ProjectReportService(ISettingService settingService, IRepository repository)
    {
        _settingService = settingService;
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<List<ProjectWorkTimeDto>> GetWorkTimesPerProject(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.Get(cancellationToken);
        var filter = ReportServiceFilter.Create(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var workedTimesPerProject = await GetWorkedTimesPerProject(filter, cancellationToken);

        var totalWorkedDays = workedTimesPerProject.Sum(x => x.WorkedDays);

        var result = workedTimesPerProject
            .Select(worked => new ProjectWorkTimeDto
            {
                ProjectId = worked.ProjectId,
                ProjectTitle = worked.ProjectTitle,
                CustomerTitle = worked.CustomerTitle,
                TimeWorked = worked.WorkedTime,
                DaysWorked = worked.WorkedDays,
                RatioTotalWorked = totalWorkedDays != 0 ? worked.WorkedDays / totalWorkedDays : 0,
                BudgetWorked = worked.WorkedBudget,
                Currency = settings.Currency,
            })
            .OrderBy(x => x.ProjectTitle)
            .ThenBy(x => x.CustomerTitle)
            .ToList();

        result = AppendCustomerToNonUniqueProjectNames(result);
        return result;
    }

    private async Task<List<ProjectWorkTime>> GetWorkedTimesPerProject(ReportServiceFilter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var workedTimesPerProjectAndOrder = await _repository
            .GetGrouped(
                groupBy: x => new { x.Project.Id, x.Project.Title, x.OrderId },
                select: x => new ProjectWorkTime
                {
                    ProjectId = x.Key.Id,
                    ProjectTitle = x.Key.Title,
                    CustomerTitle = x.FirstOrDefault().Project.Customer.Title,
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal))),
                    HourlyRate = x.Key.OrderId != null
                        ? x.Min(t => t.Order.HourlyRate)
                        : x.Min(t => t.Project.Customer.HourlyRate),
                },
                where: filter.WorkedTimes.CreateFilter(),
                cancellationToken: cancellationToken
            );

        var workedTimesPerProject = workedTimesPerProjectAndOrder
            .GroupBy(x => new { x.ProjectId, x.ProjectTitle })
            .Select(x => new ProjectWorkTime
            {
                ProjectId = x.Key.ProjectId,
                ProjectTitle = x.Key.ProjectTitle,
                CustomerTitle = x.First().CustomerTitle,
                WorkedTime = x.Sum(h => h.WorkedTime),
                HourlyRate = x.ToList().GetAverageHourlyRate(h => h.WorkedTime),
            })
            .ToList();

        foreach (var workTime in workedTimesPerProject)
            workTime.WorkedDays = workTime.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return workedTimesPerProject;
    }

    private static List<ProjectWorkTimeDto> AppendCustomerToNonUniqueProjectNames(IEnumerable<ProjectWorkTimeDto> projectWorkTimes)
        => projectWorkTimes
            .GroupBy(project => project.ProjectTitle)
            .SelectMany(projectNameGroup => projectNameGroup.Count() > 1 ? projectNameGroup.Select(AppendCustomerProjectName) : projectNameGroup)
            .ToList();

    private static ProjectWorkTimeDto AppendCustomerProjectName(ProjectWorkTimeDto projectWorkTime)
    {
        projectWorkTime.ProjectTitle = $"{projectWorkTime.ProjectTitle} ({projectWorkTime.CustomerTitle})";
        return projectWorkTime;
    }
}