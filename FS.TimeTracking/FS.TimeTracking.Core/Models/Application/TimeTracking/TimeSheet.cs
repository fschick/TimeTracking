using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Models;
using FS.TimeTracking.Core.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Activity = FS.TimeTracking.Core.Models.Application.MasterData.Activity;

namespace FS.TimeTracking.Core.Models.Application.TimeTracking;

/// <summary>
/// Time sheet position.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TimeSheet : IIdEntityModel, IUserLinkedModel, ICustomerLinkedModel
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
    /// The related issue/ticket/... .
    /// </summary>
    public string Issue { get; set; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="MasterData.Customer"/>.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc />
    Guid? ICustomerLinkedDto.CustomerId => CustomerId;

    /// <inheritdoc cref="MasterData.Project"/>
    public Customer Customer { get; set; }

    /// <summary>
    /// The identifier to the related <see cref="MasterData.Project"/>.
    /// </summary>
    public Guid? ProjectId { get; set; }

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

    /// <summary>
    /// The identifier of the user this entity belongs to.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay =>
        $"{StartDate:dd.MM.yyyy HH:mm} - {EndDate:dd.MM.yyyy HH:mm}"
        + (Customer != null ? $", {Customer.Title}" : string.Empty)
        + (Activity != null ? $", {Activity.Title}" : string.Empty);
}