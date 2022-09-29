﻿using FS.TimeTracking.Abstractions.DTOs.Reporting;
using FS.TimeTracking.Application.AutoMapper;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Reporting;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Core.Models.Filter;
using FS.TimeTracking.Report.Client.Api;
using FS.TimeTracking.Report.Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Reporting;

/// <inheritdoc />
public class ActivityReportService : IActivityReportService
{
    private readonly ISettingService _settingService;
    private readonly IRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly TimeTrackingConfiguration _configuration;
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportService"/> class.
    /// </summary>
    /// <param name="settingService">The setting service.</param>
    /// <param name="repository">The repository.</param>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="apiDescriptionGroupCollectionProvider">The API description group collection provider.</param>
    public ActivityReportService(ISettingService settingService, IRepository repository, HttpClient httpClient, IOptions<TimeTrackingConfiguration> configuration, IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
    {
        _settingService = settingService;
        _repository = repository;
        _httpClient = httpClient;
        _apiDescriptionProvider = apiDescriptionGroupCollectionProvider;
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public async Task<List<ActivityReportGridDto>> GetCustomersHavingTimeSheets(TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateTimeSheetFilter(filters);

        var activityReportUrl = _apiDescriptionProvider.ApiDescriptionGroups.Items
            .SelectMany(x => x.Items)
            .Single(x => (x.ActionDescriptor as ControllerActionDescriptor)?.ActionName == nameof(GetActivityReport))
            .RelativePath;

        var queryParams = FilterExtensions.ToQueryParams(
            filters,
                ("language", language)
            );

        var reportDownloadUrl = $"{activityReportUrl}?{queryParams}";

        var customers = await _repository
            .GetGrouped(
                groupBy: (TimeSheet x) => x.Project.CustomerId,
                select: x => new { Id = x.Key, x.FirstOrDefault().Project.Customer.Title },
                where: filter,
                orderBy: o => o.OrderBy(x => x.Title),
                cancellationToken: cancellationToken
            );

        return customers
            .Select(customer => new ActivityReportGridDto
            {
                CustomerId = customer.Id,
                CustomerTitle = customer.Title,
                DailyActivityReportUrl = $"{reportDownloadUrl}&customerId={customer.Id}&reportType={ActivityReportType.Daily.ToString().LowercaseFirstChar()}",
                DetailedActivityReportUrl = $"{reportDownloadUrl}&customerId={customer.Id}&reportType={ActivityReportType.Detailed.ToString().LowercaseFirstChar()}",
            })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<FileResult> GetActivityReport(TimeSheetFilterSet filters, string language, ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
    {
        var reportData = await GetActivityReportData(filters, language, reportType, cancellationToken);
        using var activityReportClient = new ActivityReportApi(_httpClient, _configuration.Reporting.ReportServerBaseUrl);
        var apiResponse = await activityReportClient.GenerateActivityReportWithHttpInfoAsync(reportData, cancellationToken);

        var mimeTypeHeader = apiResponse.Headers["Content-Type"].Single();
        var contentDispositionHeader = apiResponse.Headers["Content-Disposition"].Single();
        var contentDisposition = ContentDispositionHeaderValue.Parse(contentDispositionHeader);
        var fileName = contentDisposition.FileNameStar.ToString();
        if (string.IsNullOrEmpty(fileName))
            fileName = contentDisposition.FileName.ToString();
        var reportFileResult = new FileContentResult(apiResponse.Data, mimeTypeHeader)
        {
            FileDownloadName = fileName
        };

        return reportFileResult;
    }

    /// <inheritdoc />
    public async Task<ReportPreviewDto> GetActivityReportPreview(TimeSheetFilterSet filters, string language, ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
    {
        var reportData = await GetActivityReportData(filters, language, reportType, cancellationToken);
        if (reportData.TimeSheets?.Any() != true)
            return new ReportPreviewDto { Pages = null, TotalPages = 0 };
        using var activityReportClient = new ActivityReportApi(_httpClient, _configuration.Reporting.ReportServerBaseUrl);
        return await activityReportClient.GenerateActivityReportPreviewAsync(1, 3, reportData, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ActivityReportDto> GetActivityReportData(TimeSheetFilterSet filters, string language, ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
    {
        var selectedPeriod = FilterExtensions.GetSelectedPeriod(filters, true);
        var parameters = new ReportParameter
        {
            ReportType = reportType,
            Culture = language,
            StartDate = selectedPeriod.Start,
            EndDate = selectedPeriod.End,
        };

        var serviceProvider = await GetServiceProviderInformation(cancellationToken);

        var translations = await _settingService.GetTranslations(language, cancellationToken);
        var reportTranslations = translations
            .SelectToken("Page.Report.Activity")
            .ToDictionary()
            .ToDictionary(x => x.Key, x => x.Value.Trim('"'));

        var timeSheets = await GetTimeSheets(filters, cancellationToken);
        SetGroupByMember(timeSheets, reportType, reportTranslations);

        var reportData = new ActivityReportDto
        {
            Parameters = parameters,
            ServiceProvider = serviceProvider,
            Translations = reportTranslations,
            TimeSheets = timeSheets,
        };

        return reportData;
    }

    private async Task<ServiceProviderDto> GetServiceProviderInformation(CancellationToken cancellationToken)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
        var serviceProvider = new ServiceProviderDto
        {
            Name = settings.Company.ServiceProvider,
            Company = settings.Company.Company,
            Department = settings.Company.Department,
            Logo = settings.Company.Logo,
        };
        return serviceProvider;
    }

    private async Task<List<ActivityReportTimeSheetDto>> GetTimeSheets(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateTimeSheetFilter(filters);

        var timeSheets = await _repository
            .Get<TimeSheet, ActivityReportTimeSheetDto>(
                where: filter,
                orderBy: o => o.OrderBy(x => x.StartDateLocal),
                cancellationToken: cancellationToken
            );

        return timeSheets;
    }

    private static void SetGroupByMember(List<ActivityReportTimeSheetDto> timeSheets, ActivityReportType reportType, IDictionary<string, string> reportTranslations)
    {
        reportTranslations.Add("GroupByTitle", reportTranslations.FirstOrDefault(x => x.Key == reportType.ToString()).Value);
        foreach (var timeSheet in timeSheets)
            timeSheet.GroupBy = reportType switch
            {
                ActivityReportType.Detailed => null,
                ActivityReportType.Daily => null,
                _ => throw new ArgumentOutOfRangeException(nameof(reportType), reportType, null)
            };
    }
}