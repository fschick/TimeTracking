using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="Setting"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Setting))]
public record SettingDto
{
    /// <inheritdoc cref="WorkdaysOfWeekDto" />
    [Required]
    public WorkdaysOfWeekDto Workdays { get; set; } = new();

    /// <summary>
    /// The average working hours per workday
    /// </summary>
    [Required]
    public TimeSpan WorkHoursPerWorkday { get; set; } = TimeSpan.FromHours(8);

    /// <inheritdoc cref="CompanyDto" />
    [Required]
    public CompanyDto Company { get; set; } = new();

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
        /// The name of the holder/provider
        /// </summary>
        public string Provider { get; set; }

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
    }
}