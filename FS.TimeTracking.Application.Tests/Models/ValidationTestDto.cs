using FS.TimeTracking.Shared.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Application.Tests.Models
{
    [ValidationDescription]
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

        [Range(double.MinValue, 4d)]
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
    public class TestNestedOuterDto
    {
        [Required]
        public string NestedRequired { get; set; }
    }
}