﻿using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.MasterData;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking;

/// <inheritdoc cref="Customer"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Customer))]
public record CustomerDto
{
    /// <inheritdoc cref="Customer.Id"/>
    [Required]
    [Filter(Visible = false)]
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

    /// <inheritdoc cref="Customer.Hidden"/>
    [Required]
    public bool Hidden { get; init; }
}