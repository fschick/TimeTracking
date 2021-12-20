using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Customer"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Customer))]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record CustomerDto
{
    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    public Guid Id { get; init; }

    /// <inheritdoc cref="Customer.Title"/>
    [Required]
    [StringLength(100)]
    public string Title { get; init; }

    /// <inheritdoc cref="Customer.Number"/>
    public string Number { get; set; }

    /// <inheritdoc cref="Customer.Department"/>
    public string Department { get; set; }

    /// <inheritdoc cref="Customer.CompanyName"/>
    [StringLength(100)]
    public string CompanyName { get; init; }

    /// <inheritdoc cref="Customer.ContactName"/>
    [StringLength(100)]
    public string ContactName { get; init; }

    /// <inheritdoc cref="Customer.HourlyRate"/>
    [Required]
    [Range(0, double.PositiveInfinity)]
    public double HourlyRate { get; set; }

    /// <inheritdoc cref="Customer.Street"/>
    [StringLength(100)]
    public string Street { get; init; }

    /// <inheritdoc cref="Customer.ZipCode"/>
    [StringLength(100)]
    public string ZipCode { get; init; }

    /// <inheritdoc cref="Customer.City"/>
    [StringLength(100)]
    public string City { get; init; }

    /// <inheritdoc cref="Customer.Country"/>
    [StringLength(100)]
    public string Country { get; init; }

    /// <inheritdoc cref="Customer.Comment"/>
    public string Comment { get; set; }

    /// <inheritdoc cref="Customer.Hidden"/>
    [Required]
    public bool Hidden { get; init; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}