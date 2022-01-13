using FS.TimeTracking.Shared.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.Report;

/// <summary>
/// Work times for a project.
/// </summary>
public class ProjectWorkTimeDto : WorkTimeDto
{
    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid ProjectId { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}