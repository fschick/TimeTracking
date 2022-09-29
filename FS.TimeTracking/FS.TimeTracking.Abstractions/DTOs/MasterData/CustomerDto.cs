using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Customer
/// </summary>
[ValidationDescription]
[FilterEntity(Prefix = "Customer")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record CustomerDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// The display name of the customer.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; init; }

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
    public string CompanyName { get; init; }

    /// <summary>
    /// The name of the contact.
    /// </summary>
    [StringLength(100)]
    public string ContactName { get; init; }

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
    public string Street { get; init; }

    /// <summary>
    /// The zip code.
    /// </summary>
    [StringLength(100)]
    public string ZipCode { get; init; }

    /// <summary>
    /// The city.
    /// </summary>
    [StringLength(100)]
    public string City { get; init; }

    /// <summary>
    /// The country.
    /// </summary>
    [StringLength(100)]
    public string Country { get; init; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Indicates whether this item is hidden.
    /// </summary>
    [Required]
    public bool Hidden { get; init; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}