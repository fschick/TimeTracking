using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Models;

[ValidationDescription]
[ExcludeFromCodeCoverage]
public class ValidationTestDto
{
    [Required]
    public string Required { get; set; }


    [MinLength(2)]
    public string MinLength { get; set; }

    [MaxLength(4)]
    public string MaxLength { get; set; }

    [StringLength(4, MinimumLength = 2)]
    public string StringLength { get; set; }

    [StringLength(4)]
    public string StringLengthMax { get; set; }


    [Range(2, 4)]
    public int RangeInt { get; set; }

    [Range(2, int.MaxValue)]
    public int RangeIntMin { get; set; }

    [Range(int.MinValue, 4)]
    public int RangeIntMax { get; set; }

    [Range(2d, 4d)]
    public double RangeDouble { get; set; }

    [Range(2d, double.MaxValue)]
    public double RangeDoubleMin { get; set; }

    [Range(2d, double.PositiveInfinity)]
    public double RangeDoubleMinInfinity { get; set; }

    [Range(double.MinValue, 4d)]
    public double RangeDoubleMax { get; set; }

    [Range(double.NegativeInfinity, 4d)]
    public double RangeDoubleMaxInfinity { get; set; }

    [Range(typeof(DateTime), "2020-01-01", "2020-01-31")]
    public DateTime RangeDate { get; set; }

    [Range(typeof(DateTime), "2020-01-01", null)]
    public DateTime RangeDateMin { get; set; }

    [Range(typeof(DateTime), null, "2020-01-31")]
    public DateTime RangeDateMax { get; set; }


    [Compare(nameof(Required))]
    public string Compare { get; set; }

    [CompareTo(ComparisonType.Equal, nameof(Required))]
    public string CompareToEqual { get; set; }

    [CompareTo(ComparisonType.NotEqual, nameof(Required))]
    public string CompareToNotEqual { get; set; }

    [CompareTo(ComparisonType.LessThan, nameof(Required))]
    public string CompareToLessThan { get; set; }

    [CompareTo(ComparisonType.LessThanOrEqual, nameof(Required))]
    public string CompareToLessThanOrEqual { get; set; }

    [CompareTo(ComparisonType.GreaterThan, nameof(Required))]
    public string CompareToGreaterThan { get; set; }

    [CompareTo(ComparisonType.GreaterThanOrEqual, nameof(Required))]
    public string CompareToGreaterThanOrEqual { get; set; }


    [Required]
    [MinLength(4)]
    [StringLength(4, MinimumLength = 2)]
    public string MultiValidation { get; set; }


    public TestNestedInnerDto NestedOuter { get; set; }

    public TestNestedOuterDto NestedInner { get; set; }


    [ValidationDescription]
    public class TestNestedInnerDto
    {
        [Required]
        public string NestedRequired { get; set; }
    }
}

[ValidationDescription]
[ExcludeFromCodeCoverage]
public class TestNestedOuterDto
{
    [Required]
    public string NestedRequired { get; set; }
}