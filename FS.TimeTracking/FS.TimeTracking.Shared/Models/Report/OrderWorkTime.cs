using FS.TimeTracking.Shared.Models.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.Report;

/// <summary>
/// Work times per order.
/// </summary>
public class OrderWorkTime : WorkTime
{
    /// <inheritdoc cref="Order.Id"/>
    [Required]
    public Guid OrderId { get; set; }

    /// <inheritdoc cref="Order.Title"/>
    [Required]
    public string OrderTitle { get; set; }

    /// <inheritdoc cref="Order.Number"/>
    [Required]
    public string OrderNumber { get; set; }

    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }
}