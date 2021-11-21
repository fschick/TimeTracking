using FS.TimeTracking.Shared.Models.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Order"/>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class OrderListDto
{
    /// <inheritdoc cref="Order.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Order.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="Order.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="Order.DueDate"/>
    [Required]
    public DateTimeOffset DueDate { get; set; }

    /// <inheritdoc cref="Order.Hidden"/>
    public bool Hidden { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}