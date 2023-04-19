using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.MasterData;

/// <summary>
/// Order
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class Order : IIdEntityModel, ICustomerLinkedModel
{
    /// <inheritdoc />
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
    /// The identifier to the related <see cref="MasterData.Customer"/>.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc />
    Guid? ICustomerLinkedDto.CustomerId => CustomerId;

    /// <inheritdoc cref="MasterData.Customer"/>
    public Customer Customer { get; set; }

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
    /// The due date in local time.
    /// </summary>
    public DateTime DueDateLocal { get; set; }

    /// <summary>
    /// The due date's timezone offset in hours.
    /// </summary>
    public int DueDateOffset { get; set; }

    /// <summary>
    /// The due date.
    /// </summary>
    [Required]
    [NotMapped]
    [CompareTo(ComparisonType.GreaterThanOrEqual, nameof(StartDate))]
    public DateTimeOffset DueDate
    {
        get => DueDateLocal.ToOffset(TimeSpan.FromMinutes(DueDateOffset));
        set { DueDateLocal = value.DateTime; DueDateOffset = (int)value.Offset.TotalMinutes; }
    }

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
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({Number})";
}