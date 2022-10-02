using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.Chart;

/// <summary>
/// Work times per order.
/// </summary>
[ExcludeFromCodeCoverage]
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

    /// <inheritdoc cref="Order.Hidden"/>
    [Required]
    public bool OrderHidden { get; set; }

    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="Customer.Hidden"/>
    [Required]
    public bool CustomerHidden { get; set; }
}