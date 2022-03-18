using FS.TimeTracking.Report.Abstractions.Interfaces.Application.Services.Report;
using FS.TimeTracking.Report.Abstractions.Models.Configuration;
using FS.TimeTracking.Shared.DTOs.Report;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stimulsoft.Base;
using Stimulsoft.Report;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Report.Application.Services.Report;

/// <inheritdoc />
public class TimeSheetReportService : ITimeSheetReportService
{
    private readonly TimeTrackingReportConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSheetReportService"/> class.
    /// </summary>
    public TimeSheetReportService(IOptions<TimeTrackingReportConfiguration> configuration)
        => _configuration = configuration.Value;

    /// <inheritdoc />
    public Task<StiReport> GetTimeSheetReport(TimeSheetReportDto source, CancellationToken cancellationToken = default)
    {
        Stimulsoft.Base.StiLicense.Key = _configuration.StimulsoftLicenseKey;
        var report = StiReport.CreateNewReport();
        report.Load(source.Report);
        report.Dictionary.Databases.Clear();

        var reportDataJson = JsonConvert.SerializeObject(source.Data);
        using var reportData = StiJsonToDataSetConverterV2.GetDataSet(reportDataJson);
        report.RegData("TimeSheet", reportData);

        var customers = source.Data.TimeSheets.Select(x => x.CustomerTitle).Distinct().OrderBy(x => x);
        report.ReportName = $"{source.Data.Translations["Title"]} - {source.Data.Parameters.StartDate:yyyy-MM-dd} - {source.Data.Parameters.EndDate:yyyy-MM-dd} - {string.Join(", ", customers)}";

        return Task.FromResult(report);
    }
}