using FS.TimeTracking.Shared.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Application.Tests
{
    [ValidationDescription]
    public class TestDto
    {
        [Required]
        public string Required { get; set; }


        [MinLength(5)]
        public string MinLength { get; set; }

        [MaxLength(5)]
        public string MaxLength { get; set; }

        [StringLength(5, MinimumLength = 1)]
        public string StringLength { get; set; }

        [StringLength(5)]
        public string StringLengthMax { get; set; }


        [Range(1, 5)]
        public int RangeInt { get; set; }

        [Range(1, int.MaxValue)]
        public int RangeIntMin { get; set; }

        [Range(int.MinValue, 5)]
        public int RangeIntMax { get; set; }

        [Range(1d, 5d)]
        public double RangeDouble { get; set; }

        [Range(1d, double.MaxValue)]
        public double RangeDoubleMin { get; set; }

        [Range(double.MinValue, 5d)]
        public double RangeDoubleMax { get; set; }

        [Range(typeof(DateTime), "2020-01-01", "2020-01-31")]
        public DateTime RangeDate { get; set; }

        [Range(typeof(DateTime), "2020-01-01", null)]
        public DateTime RangeDateMin { get; set; }

        [Range(typeof(DateTime), null, "2020-01-31")]
        public DateTime RangeDateMax { get; set; }


        [Compare(nameof(Required))]
        public string Compare { get; set; }


        [Required]
        [MinLength(5)]
        [StringLength(5, MinimumLength = 1)]
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
    public class TestNestedOuterDto
    {
        [Required]
        public string NestedRequired { get; set; }
    }
}