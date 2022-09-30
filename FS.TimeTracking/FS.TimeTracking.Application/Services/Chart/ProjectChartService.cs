﻿using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.Chart;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Chart;

/// <inheritdoc />
public class ProjectChartService : IProjectChartService
{
    private readonly ISettingService _settingService;
    private readonly IRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectChartService" /> class.
    /// </summary>
    /// <param name="settingService">The setting service.</param>
    /// <param name="repository">The repository.</param>
    /// <autogeneratedoc />
    public ProjectChartService(ISettingService settingService, IRepository repository)
    {
        _settingService = settingService;
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<List<ProjectWorkTimeDto>> GetWorkTimesPerProject(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
        var filter = ChartFilter.Create(filters);
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
                TotalWorkedPercentage = totalWorkedDays != 0 ? worked.WorkedDays / totalWorkedDays : 0,
                BudgetWorked = worked.WorkedBudget,
                Currency = settings.Company.Currency,
            })
            .OrderBy(x => x.ProjectTitle)
            .ThenBy(x => x.CustomerTitle)
            .ToList();

        result = AppendCustomerToNonUniqueProjectNames(result);
        return result;
    }

    private async Task<List<ProjectWorkTime>> GetWorkedTimesPerProject(ChartFilter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.GetSettings(cancellationToken);

        var timeSheetsPerProjectAndOrder = await _repository
            .GetGrouped(
                groupBy: timeSheet => new { timeSheet.Project.Id, timeSheet.Project.Title, timeSheet.OrderId },
                select: timeSheets => new
                {
                    ProjectId = timeSheets.Key.Id,
                    ProjectTitle = timeSheets.Key.Title,
                    CustomerTitle = timeSheets.FirstOrDefault().Project.Customer.Title,
                    WorkedTime = TimeSpan.FromSeconds(timeSheets.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal, f.EndDateOffset))),
                    HourlyRate = timeSheets.Key.OrderId != null
                        ? timeSheets.Min(t => t.Order.HourlyRate)
                        : timeSheets.Min(t => t.Project.Customer.HourlyRate),
                },
                where: filter.WorkedTimes.CreateFilter(),
                cancellationToken: cancellationToken
            );

        var workedTimesPerProject = timeSheetsPerProjectAndOrder
            .GroupBy(timeSheet => new { timeSheet.ProjectId, timeSheet.ProjectTitle })
            .Select(timeSheets => new ProjectWorkTime
            {
                ProjectId = timeSheets.Key.ProjectId,
                ProjectTitle = timeSheets.Key.ProjectTitle,
                CustomerTitle = timeSheets.First().CustomerTitle,
                WorkedTime = timeSheets.Sum(h => h.WorkedTime),
                WorkedBudget = timeSheets.Select(f => f.WorkedTime.TotalHours * f.HourlyRate).Sum(),
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