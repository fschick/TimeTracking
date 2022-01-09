using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Enums;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Models;
using Newtonsoft.Json;

namespace FS.TimeTracking.Shared.Models.Application.MasterData;

/// <summary>
/// Holiday
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class Holiday : IIdEntityModel
{
    /// <inheritdoc />
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

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
    public DateTime EndDateLocal { get; set; }

    /// <summary>
    /// The end date's timezone offset in hours.
    /// </summary>
    public int EndDateOffset { get; set; }

    /// <summary>
    /// The end date.
    /// </summary>
    [Required]
    [NotMapped]
    [CompareTo(Shared.ComparisonType.GreaterThanOrEqual, nameof(StartDate))]
    public DateTimeOffset EndDate
    {
        get => EndDateLocal.ToOffset(TimeSpan.FromMinutes(EndDateOffset));
        set { EndDateLocal = value.DateTime; EndDateOffset = (int)value.Offset.TotalMinutes; }
    }

    /// <summary>
    /// The reason for holiday.
    /// </summary>
    [Required]
    public HolidayType Type { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({StartDate:d} - {EndDate:d})";
}