using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Enums.Report;
using FS.TimeTracking.Application.AutoMapper;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Report;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Report.Client.Api;
using FS.TimeTracking.Report.Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Report;

/// <inheritdoc />
public class ActivityReportService : IActivityReportService
{
    private readonly ISettingService _settingService;
    private readonly IRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly TimeTrackingConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportService"/> class.
    /// </summary>
    /// <param name="settingService">The setting service.</param>
    /// <param name="repository">The repository.</param>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    public ActivityReportService(ISettingService settingService, IRepository repository, HttpClient httpClient, IOptions<TimeTrackingConfiguration> configuration)
    {
        _settingService = settingService;
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public async Task<FileResult> GetDetailedActivityReport(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, string language, ActivityReportGroup groupBy, CancellationToken cancellationToken = default)
    {
        var reportData = await GetActivityReportData(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, groupBy, cancellationToken);
        using var activityReportClient = new ActivityReportApi(_httpClient, _configuration.Report.ReportServerBaseUrl);
        var apiResponse = await activityReportClient.GenerateActivityReportWithHttpInfoAsync(reportData, cancellationToken);

        var mimeType = apiResponse.Headers["Content-Type"].Single();
        var contentDisposition = apiResponse.Headers["Content-Disposition"].Single();
        var fileName = Regex.Match(contentDisposition, "filename=\"(?<filename>.*?)\"").Groups["filename"].Value;
        var reportFileResult = new FileContentResult(apiResponse.Data, mimeType)
        {
            FileDownloadName = fileName
        };

        return reportFileResult;
    }

    /// <inheritdoc />
    public async Task<ReportPreviewDto> GetDetailedActivityReportPreview(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, string language, ActivityReportGroup groupBy, CancellationToken cancellationToken = default)
    {
        var reportData = await GetActivityReportData(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, language, groupBy, cancellationToken);
        if (reportData.TimeSheets?.Any() != true)
            return new ReportPreviewDto { Pages = null, TotalPages = 0 };
        using var activityReportClient = new ActivityReportApi(_httpClient, _configuration.Report.ReportServerBaseUrl);
        return await activityReportClient.GenerateActivityReportPreviewAsync(1, 3, reportData, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ActivityReportDto> GetActivityReportData(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, string language, ActivityReportGroup groupBy, CancellationToken cancellationToken = default)
    {
        var selectedPeriod = FilterExtensions.GetSelectedPeriod(timeSheetFilter, true);
        var parameters = new ReportParameter { StartDate = selectedPeriod.Start, EndDate = selectedPeriod.End };

        var provider = await GetProviderInformation(cancellationToken);

        var translations = await _settingService.GetTranslations(language, cancellationToken);
        var reportTranslations = translations
            .SelectToken("Page.Report.Activity")
            .ToDictionary()
            .ToDictionary(x => x.Key, x => x.Value.Trim('"'));

        var timeSheets = await GetTimeSheets(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);
        SetGroupByMember(timeSheets, groupBy, reportTranslations);

        var reportData = new ActivityReportDto
        {
            Parameters = parameters,
            Provider = provider,
            Translations = reportTranslations,
            TimeSheets = timeSheets,
        };

        return reportData;
    }

    private async Task<ProviderDto> GetProviderInformation(CancellationToken cancellationToken)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
        var provider = new ProviderDto
        {
            Name = settings.Company.Provider,
            Company = settings.Company.Company,
            Department = settings.Company.Department,
        };
        return provider;
    }

    private async Task<List<ActivityReportTimeSheetDto>> GetTimeSheets(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateTimeSheetFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);

        var timeSheets = await _repository
            .Get<TimeSheet, ActivityReportTimeSheetDto>(
                where: filter,
                orderBy: o => o.OrderBy(x => x.StartDateLocal),
                cancellationToken: cancellationToken
            );

        return timeSheets;
    }

    private static void SetGroupByMember(List<ActivityReportTimeSheetDto> timeSheets, ActivityReportGroup groupBy, Dictionary<string, string> reportTranslations)
    {
        reportTranslations.Add("GroupByTitle", reportTranslations.FirstOrDefault(x => x.Key == groupBy.ToString()).Value);
        foreach (var timeSheet in timeSheets)
            timeSheet.GroupBy = groupBy switch
            {
                ActivityReportGroup.None => null,
                ActivityReportGroup.Issue => timeSheet.Issue,
                ActivityReportGroup.OrderNumber => timeSheet.OrderNumber,
                _ => throw new ArgumentOutOfRangeException(nameof(groupBy), groupBy, null)
            };
    }
}