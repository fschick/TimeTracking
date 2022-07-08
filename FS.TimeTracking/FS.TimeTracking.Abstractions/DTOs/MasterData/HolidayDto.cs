using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Core.Attributes;
using FS.TimeTracking.Core.Models.Shared;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Holiday
/// </summary>
[ValidationDescription]
[FilterEntity(Prefix = "Holiday")]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record HolidayDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// The display name of the holiday.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; init; }

    /// <summary>
    /// The start date.
    /// </summary>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// The end date.
    /// </summary>
    [Required]
    [CompareTo(ComparisonType.GreaterThanOrEqual, nameof(StartDate))]
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// The reason for holiday.
    /// </summary>
    [Required]
    public HolidayType Type { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({StartDate:d} - {EndDate:d})";
}