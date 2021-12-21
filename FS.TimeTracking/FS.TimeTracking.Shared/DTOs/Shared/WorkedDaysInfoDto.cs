namespace FS.TimeTracking.Shared.DTOs.Shared;

/// <summary>
/// Workday counts.
/// </summary>
public class WorkedDaysInfoDto
{
    /// <summary>
    /// Count of public workdays.
    /// </summary>
    public double PublicWorkdays { get; set; }

    /// <summary>
    /// Count of public workdays + individual leave days.
    /// </summary>
    public double PersonalWorkdays { get; set; }
}