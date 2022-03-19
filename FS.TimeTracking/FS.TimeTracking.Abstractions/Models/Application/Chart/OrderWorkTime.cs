﻿using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.Models.Application.Chart;

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