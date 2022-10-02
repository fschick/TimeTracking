using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.Chart;

/// <summary>
/// Work times per activity.
/// </summary>
[ExcludeFromCodeCoverage]
public class ActivityWorkTime : WorkTime
{
    /// <inheritdoc cref="Activity.Id"/>
    [Required]
    public Guid ActivityId { get; set; }

    /// <inheritdoc cref="Activity.Title"/>
    [Required]
    public string ActivityTitle { get; set; }

    /// <inheritdoc cref="Activity.Hidden"/>
    [Required]
    public bool ActivityHidden { get; set; }
}