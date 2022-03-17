using FS.TimeTracking.Abstractions.Extensions;
using FS.TimeTracking.Abstractions.Interfaces.Models;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.TimeTracking.Abstractions.Models.Application.TimeTracking;

/// <summary>
/// Time sheet position.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TimeSheet : IIdEntityModel
{
    /// <inheritdoc />
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The start date in local time.
    /// </summary>
    public DateTime StartDateLocal { get; set; }

    /// <summary>
    /// The start date's timezone offset in hours.
    /// </summary>
    public int StartDateOffset { get; set; }

    /// <summary>
    /// The start date.
    /// </summary>
    [Required]
    [NotMapped]
    public DateTimeOffset StartDate
    {
        get => StartDateLocal.ToOffset(TimeSpan.FromMinutes(StartDateOffset));
        set { StartDateLocal = value.DateTime; StartDateOffset = (int)value.Offset.TotalMinutes; }
    }

    /// <summary>
    /// The end date in UTC.
    /// </summary>
    public DateTime? EndDateLocal { get; set; }

    /// <summary>
    /// The end date's timezone offset in hours.
    /// </summary>
    public int? EndDateOffset { get; set; }

    /// <summary>
    /// The end date.
    /// </summary>
    [NotMapped]
    public DateTimeOffset? EndDate
    {
        get => EndDateLocal?.ToOffset(TimeSpan.FromMinutes(EndDateOffset!.Value));
        set { EndDateLocal = value?.DateTime; EndDateOffset = (int?)value?.Offset.TotalMinutes; }
    }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// The related issue/ticket/... .
    /// </summary>
    public string Issue { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="MasterData.Project"/>.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <inheritdoc cref="MasterData.Project"/>
    public Project Project { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="MasterData.Activity"/>.
    /// </summary>
    [Required]
    public Guid ActivityId { get; set; }

    /// <inheritdoc cref="MasterData.Activity"/>
    public Activity Activity { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="MasterData.Order"/>.
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <inheritdoc cref="MasterData.Order"/>
    public Order Order { get; set; }

    /// <summary>
    /// Indicates whether this item is billable.
    /// </summary>
    [Required]
    public bool Billable { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }

    [JsonIgnore]
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{StartDate:d} - {EndDate:d}"
        + (Project?.Customer != null ? $", {Project.Customer.Title}" : string.Empty)
        + (Project != null ? $", {Project.Title}" : string.Empty)
        + (Activity != null ? $", {Activity.Title}" : string.Empty);
}