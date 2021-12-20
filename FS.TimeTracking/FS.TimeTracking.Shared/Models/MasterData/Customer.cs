using FS.TimeTracking.Shared.Interfaces.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Shared.Models.MasterData;

/// <summary>
/// Customer
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class Customer : IIdEntityModel
{
    /// <inheritdoc />
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// The customer number.
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// The department.
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// The name of the company.
    /// </summary>
    [StringLength(100)]
    public string CompanyName { get; set; }

    /// <summary>
    /// The name of the contact.
    /// </summary>
    [StringLength(100)]
    public string ContactName { get; set; }

    /// <summary>
    /// The hourly rate.
    /// </summary>
    [Required]
    [Range(0, double.PositiveInfinity)]
    public double HourlyRate { get; set; }

    /// <summary>
    /// The street.
    /// </summary>
    [StringLength(100)]
    public string Street { get; set; }

    /// <summary>
    /// The zip code.
    /// </summary>
    [StringLength(100)]
    public string ZipCode { get; set; }

    /// <summary>
    /// The city.
    /// </summary>
    [StringLength(100)]
    public string City { get; set; }

    /// <summary>
    /// The country.
    /// </summary>
    [StringLength(100)]
    public string Country { get; set; }

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

    /// <summary>
    /// The projects related to this customer.
    /// </summary>
    public List<Project> Projects { get; set; }

    /// <summary>
    /// The orders related to this customer.
    /// </summary>
    public List<Order> Orders { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}