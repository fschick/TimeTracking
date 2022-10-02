using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.Chart;

/// <summary>
/// Work times per customer.
/// </summary>
[ExcludeFromCodeCoverage]
public class CustomerWorkTime : WorkTime
{
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