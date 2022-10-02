using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.Chart;

/// <summary>
/// Work times per project.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProjectWorkTime : WorkTime
{
    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid ProjectId { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public bool ProjectHidden { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}