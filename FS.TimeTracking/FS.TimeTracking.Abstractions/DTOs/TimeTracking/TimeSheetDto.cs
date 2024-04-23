using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using Plainquire.Filter.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.TimeTracking;

/// <summary>
/// Time sheet position.
/// </summary>
[ValidationDescription]
[EntityFilter(Prefix = "TimeSheet")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record TimeSheetDto : IIdEntityDto, IManageableDto, IUserLinkedDto, ICustomerLinkedDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="CustomerDto"/>.
    /// </summary>
    [Required]
    [Filter(Filterable = false)]
    public Guid CustomerId { get; set; }

    /// <inheritdoc />
    Guid? ICustomerLinkedDto.CustomerId => CustomerId;

    /// <summary>
    /// The identifier to the related <see cref="ActivityDto"/>.
    /// </summary>
    [Required]
    [Filter(Filterable = false)]
    public Guid ActivityId { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="ProjectDto"/>.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="OrderDto"/>.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid? OrderId { get; set; }

    /// <summary>
    /// The related issue/ticket/... .
    /// </summary>
    public string Issue { get; set; }

    /// <summary>
    /// The start date.
    /// </summary>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// The end date.
    /// </summary>
    [CompareTo(ComparisonType.GreaterThan, nameof(StartDate))]
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Indicates whether this item is billable.
    /// </summary>
    [Required]
    public bool Billable { get; set; }

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
    private string DebuggerDisplay => $"{StartDate:dd.MM.yyyy HH:mm} - {EndDate:dd.MM.yyyy HH:mm}";
}