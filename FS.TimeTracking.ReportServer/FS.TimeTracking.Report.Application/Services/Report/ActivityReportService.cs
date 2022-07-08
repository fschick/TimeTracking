using FS.TimeTracking.Report.Abstractions.DTOs.Reports;
using FS.TimeTracking.Report.Core.Interfaces.Application.Services.Report;
using FS.TimeTracking.Report.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stimulsoft.Base;
using Stimulsoft.Report;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Report.Application.Services.Report;

/// <inheritdoc />
public class ActivityReportService : IActivityReportService
{
    private readonly TimeTrackingReportConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportService"/> class.
    /// </summary>
    public ActivityReportService(IOptions<TimeTrackingReportConfiguration> configuration)
        => _configuration = configuration.Value;

    /// <inheritdoc />
    public Task<StiReport> GetActivityReport(ActivityReportDto reportDto, CancellationToken cancellationToken = default)
    {
        Stimulsoft.Base.StiLicense.Key = _configuration.StimulsoftLicenseKey;
        var report = StiReport.CreateNewReport();

        var reportFolder = Path.Combine(TimeTrackingReportConfiguration.ExecutablePath, TimeTrackingReportConfiguration.REPORT_FOLDER);
        var reportFile = Path.Combine(reportFolder, "ActivityReport.Detailed.mrt");
        report.Load(reportFile);
        report.Dictionary.Databases.Clear();

        var reportDataJson = JsonConvert.SerializeObject(reportDto);
        using var reportData = StiJsonToDataSetConverterV2.GetDataSet(reportDataJson);
        report.RegData("TimeSheet", reportData);

        var customers = reportDto.TimeSheets.Select(x => x.CustomerTitle).Distinct().OrderBy(x => x);
        report.ReportName = $"{reportDto.Translations["Title"]} - {reportDto.Parameters.StartDate:yyyy-MM-dd} - {reportDto.Parameters.EndDate:yyyy-MM-dd} - {string.Join(", ", customers)}";

        return Task.FromResult(report);
    }
}