using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Enums;
using FS.TimeTracking.Shared.Models.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Holiday"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Holiday))]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record HolidayDto
{
    /// <inheritdoc cref="Holiday.Id"/>
    [Required]
    [Filter(Visible = false)]
    public Guid Id { get; init; }

    /// <inheritdoc cref="Holiday.Title"/>
    [Required]
    [StringLength(100)]
    public string Title { get; init; }

    /// <inheritdoc cref="Holiday.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="Holiday.EndDate"/>
    [Required]
    [CompareTo(Models.Shared.ComparisonType.GreaterThanOrEqual, nameof(StartDate))]
    public DateTimeOffset EndDate { get; set; }

    /// <inheritdoc cref="Holiday.Type"/>
    [Required]
    public HolidayType Type { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({StartDate:d} - {EndDate:d})";
}