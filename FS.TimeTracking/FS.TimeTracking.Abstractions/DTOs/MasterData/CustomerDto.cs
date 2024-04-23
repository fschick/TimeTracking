using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using Plainquire.Filter.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Customer
/// </summary>
[ValidationDescription]
[EntityFilter(Prefix = "Customer")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record CustomerDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Id"/>
    [JsonIgnore]
    Guid? ICustomerLinkedDto.CustomerId => Id;

    /// <summary>
    /// The display name of the customer.
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
    [Filter(Filterable = false)]
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}