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
/// Order
/// </summary>
[ValidationDescription]
[EntityFilter(Prefix = "Order")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record OrderDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the order.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Description of this item.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The order number.
    /// </summary>
    [StringLength(100)]
    public string Number { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="CustomerDto"/>.
    /// </summary>
    [Required]
    [Filter(Filterable = false)]
    public Guid CustomerId { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    Guid? ICustomerLinkedDto.CustomerId => CustomerId;

    /// <summary>
    /// The start date.
    /// </summary>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// The due date.
    /// </summary>
    [Required]
    [CompareTo(ComparisonType.GreaterThanOrEqual, nameof(StartDate))]
    public DateTimeOffset DueDate { get; set; }

    /// <summary>
    /// The hourly rate.
    /// </summary>
    [Required]
    [Range(0, double.PositiveInfinity)]
    public double HourlyRate { get; set; }

    /// <summary>
    /// The available budget.
    /// </summary>
    [Required]
    [Range(0, double.PositiveInfinity)]
    public double Budget { get; set; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Indicates whether this item is hidden.
    /// </summary>
    [Required]
    public bool Hidden { get; set; }

    /// <inheritdoc />
    [Filter(Filterable = false)]
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({Number})";
}