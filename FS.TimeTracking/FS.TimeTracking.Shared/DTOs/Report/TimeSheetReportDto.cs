namespace FS.TimeTracking.Shared.DTOs.Report;

/// <summary>
/// Time sheet report.
/// </summary>
public class TimeSheetReportDto
{
    /// <summary>
    /// Gets or sets the Stimulsoft report as binary data.
    /// </summary>
    public byte[] Report { get; set; }

    /// <summary>
    /// Gets or sets the logo to use.
    /// </summary>
    public byte[] Logo { get; set; }

    /// <summary>
    /// Gets or sets the report data.
    /// </summary>
    public TimeSheetReportDataDto Data { get; set; }
}