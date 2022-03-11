using System.Collections.Generic;

namespace FS.TimeTracking.Shared.DTOs.Report;

/// <summary>
/// Time sheet report data.
/// </summary>
public class ActivityReportDto
{
    /// <inheritdoc cref="ReportParameter" />
    public ReportParameter Parameters { get; set; }

    /// <inheritdoc cref="ProviderDto" />
    public ProviderDto Provider { get; set; }

    /// <summary>
    /// Gets or sets the translations.
    /// </summary>
    public Dictionary<string, string> Translations { get; set; }

    /// <summary>
    /// Gets or sets the time sheets.
    /// </summary>
    public List<ActivityReportGridDto> TimeSheets { get; set; }
}