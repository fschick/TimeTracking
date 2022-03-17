using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Abstractions.Models.Application.TimeTracking;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

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