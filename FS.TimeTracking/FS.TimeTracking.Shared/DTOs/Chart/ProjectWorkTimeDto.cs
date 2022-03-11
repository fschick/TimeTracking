using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.Application.MasterData;
using FS.TimeTracking.Shared.Models.Application.TimeTracking;

namespace FS.TimeTracking.Shared.DTOs.Chart;

/// <summary>
/// Work times for a project.
/// </summary>
public class IssueWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="TimeSheet.Issue"/>
    [Required]
    public string Issue { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}