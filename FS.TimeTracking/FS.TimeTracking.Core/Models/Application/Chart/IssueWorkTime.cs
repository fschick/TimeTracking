using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.Chart;

/// <summary>
/// Work times per issue.
/// </summary>
[ExcludeFromCodeCoverage]
public class IssueWorkTime : WorkTime
{
    /// <inheritdoc cref="TimeSheet.Issue"/>
    [Required]
    public string Issue { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}