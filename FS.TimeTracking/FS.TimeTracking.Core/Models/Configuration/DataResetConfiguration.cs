namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Periodic database reset configuration.
/// </summary>
public class DataResetConfiguration
{
    /// <summary>
    /// Enable periodic database reset for demo purposes.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Run database reset by cron job expression. See https://crontab.guru/ for details.
    /// </summary>
    public string CronJobSchedule { get; set; } = null!;

    /// <summary>
    ///  Path to exported data file to import on database reset.
    /// </summary>
    public string TestDatabaseSource { get; set; }

    /// <summary>
    /// Update timestamps to current time on database reset
    /// </summary>
    public bool AdjustTimeStamps { get; set; }
}