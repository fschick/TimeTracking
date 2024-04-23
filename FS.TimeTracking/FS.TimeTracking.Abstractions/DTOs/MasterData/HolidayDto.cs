using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using Plainquire.Filter.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Holiday
/// </summary>
[ValidationDescription]
[EntityFilter(Prefix = "Holiday")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record HolidayDto : IIdEntityDto, IManageableDto, IUserLinkedDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the holiday.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

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

    /// <summary>
    /// The identifier of the user this entity belongs to.
    /// </summary>
    [Required]
    [Filter(Filterable = false)]
    public Guid UserId { get; set; }

    /// <inheritdoc />
    [Filter(Filterable = false)]
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({StartDate:d} - {EndDate:d})";
}