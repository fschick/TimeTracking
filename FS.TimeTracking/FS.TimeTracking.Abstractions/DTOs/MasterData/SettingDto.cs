using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Attributes.Validation;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// User defined application settings
/// </summary>
[ValidationDescription]
[EntityFilter(Prefix = "Setting")]
[ExcludeFromCodeCoverage]
public record SettingDto : IManageableDto
{
    /// <summary>
    /// The average working hours per workday
    /// </summary>
    [Required]
    public TimeSpan WorkHoursPerWorkday { get; set; } = TimeSpan.FromHours(8);

    /// <inheritdoc cref="WorkdaysOfWeekDto" />
    [Required]
    public WorkdaysOfWeekDto Workdays { get; set; } = new();

    /// <summary>
    /// Gets or sets the client time zone identifier.
    /// </summary>
    [Required]
    public string ClientTimeZoneId { get; set; } = TimeZoneInfo.Local.Id;

    /// <inheritdoc cref="CompanyDto" />
    [Required]
    public CompanyDto Company { get; set; } = new();

    /// <inheritdoc />
    [Filter(Filterable = false)]
    public bool? IsReadonly { get; set; }

    /// <summary>
    /// Workdays of the week
    /// </summary>
    [ValidationDescription]
    public class WorkdaysOfWeekDto
    {
        /// <summary>
        /// Gets or sets whether monday should be handled as workday.
        /// </summary>
        [Required]
        public bool Monday { get; set; } = true;

        /// <summary>
        /// Gets or sets whether tuesday should be handled as workday.
        /// </summary>
        [Required]
        public bool Tuesday { get; set; } = true;

        /// <summary>
        /// Gets or sets whether wednesday should be handled as workday.
        /// </summary>
        [Required]
        public bool Wednesday { get; set; } = true;

        /// <summary>
        /// Gets or sets whether thursday should be handled as workday.
        /// </summary>
        [Required]
        public bool Thursday { get; set; } = true;

        /// <summary>
        /// Gets or sets whether friday should be handled as workday.
        /// </summary>
        [Required]
        public bool Friday { get; set; } = true;

        /// <summary>
        /// Gets or sets whether saturday should be handled as workday.
        /// </summary>
        [Required]
        public bool Saturday { get; set; }

        /// <summary>
        /// Gets or sets whether sunday should be handled as workday.
        /// </summary>
        [Required]
        public bool Sunday { get; set; }

        /// <summary>
        /// Returns workdays as dictionary of workday/value pairs.
        /// </summary>
        /// <returns></returns>
        public Dictionary<DayOfWeek, bool> AsDictionary()
            => new()
            {
                { DayOfWeek.Monday, Monday },
                { DayOfWeek.Tuesday, Tuesday },
                { DayOfWeek.Wednesday, Wednesday },
                { DayOfWeek.Thursday, Thursday },
                { DayOfWeek.Friday, Friday },
                { DayOfWeek.Saturday, Saturday },
                { DayOfWeek.Sunday, Sunday },
            };
    }

    /// <summary>
    /// Company information
    /// </summary>
    [ValidationDescription]
    public class CompanyDto
    {
        /// <summary>
        /// The name of the company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// The department of the company
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// The name of the person providing the services
        /// </summary>
        public string ServiceProvider { get; set; }

        /// <summary>
        /// The street
        /// </summary>
        [MaxLength(100)]
        public string Street { get; set; }

        /// <summary>
        /// Additional street information
        /// </summary>
        [MaxLength(100)]
        public string AddressAddition { get; set; }

        /// <summary>
        /// The postal code
        /// </summary>
        [MaxLength(100)]
        public string ZipCode { get; set; }

        /// <summary>
        /// The city
        /// </summary>
        [MaxLength(100)]
        public string City { get; set; }

        /// <summary>
        /// The country
        /// </summary>
        [MaxLength(100)]
        public string Country { get; set; }

        /// <summary>
        /// The currency to use
        /// </summary>
        [Required]
        public string Currency { get; set; } = "€";

        /// <summary>
        /// Gets or sets the company logo.
        /// </summary>
        [Image(MaxFileSize = 2 * 1024 * 1024 /*2 MB */, MaxImageWidth = 2000, MaxImageHeight = 2000)]
        public byte[] Logo { get; set; }
    }
}