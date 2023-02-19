using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Chart;

/// <summary>
/// Work times for a project.
/// </summary>
[ExcludeFromCodeCoverage]
public record IssueWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="TimeSheetDto.Issue"/>
    [Required]
    public string Issue { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}