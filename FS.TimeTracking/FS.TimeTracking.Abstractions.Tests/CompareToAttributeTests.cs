using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class CompareToAttributeTests
{
    [DataTestMethod]
    [DynamicData(nameof(GetValidNumericalComparison), DynamicDataSourceType.Method)]
    public void WhenValidComparisonOfIntIsDone_NoExceptionIsThrown(ComparisonType comparisonType, int value, int otherValue)
    {
        var compareAttribute = new CompareToAttribute(comparisonType, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = otherValue });
        Action validate = () => compareAttribute.Validate(value, ctx);
        validate.Should().NotThrow();
    }

    [DataTestMethod]
    [DynamicData(nameof(GetValidNumericalComparison), DynamicDataSourceType.Method)]
    public void WhenValidComparisonOfDoubleIsDone_NoExceptionIsThrown(ComparisonType comparisonType, double value, double otherValue)
    {
        var compareAttribute = new CompareToAttribute(comparisonType, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = otherValue });
        Action validate = () => compareAttribute.Validate(value, ctx);
        validate.Should().NotThrow();
    }

    [DataTestMethod]
    [DynamicData(nameof(GetValidDateTimeComparison), DynamicDataSourceType.Method)]
    public void WhenValidDateTimeComparisonIsDone_NoExceptionIsThrown(ComparisonType comparisonType, DateTime value, DateTime otherValue)
    {
        var compareAttribute = new CompareToAttribute(comparisonType, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = otherValue });
        Action validate = () => compareAttribute.Validate(value, ctx);
        validate.Should().NotThrow();
    }
    [DataTestMethod]
    [DynamicData(nameof(GetInvalidNumericalComparison), DynamicDataSourceType.Method)]
    public void WhenInvalidComparisonOfIntIsDone_NoExceptionIsThrown(ComparisonType comparisonType, int value, int otherValue)
    {
        var compareAttribute = new CompareToAttribute(comparisonType, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = otherValue });
        Action validate = () => compareAttribute.Validate(value, ctx);
        validate.Should().Throw<ValidationException>();
    }

    [DataTestMethod]
    [DynamicData(nameof(GetInvalidNumericalComparison), DynamicDataSourceType.Method)]
    public void WhenInvalidComparisonOfDoubleIsDone_NoExceptionIsThrown(ComparisonType comparisonType, double value, double otherValue)
    {
        var compareAttribute = new CompareToAttribute(comparisonType, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = otherValue });
        Action validate = () => compareAttribute.Validate(value, ctx);
        validate.Should().Throw<ValidationException>();
    }

    [DataTestMethod]
    [DynamicData(nameof(GetInvalidDateTimeComparison), DynamicDataSourceType.Method)]
    public void WhenInvalidDateTimeComparisonIsDone_NoExceptionIsThrown(ComparisonType comparisonType, DateTime value, DateTime otherValue)
    {
        var compareAttribute = new CompareToAttribute(comparisonType, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = otherValue });
        Action validate = () => compareAttribute.Validate(value, ctx);
        validate.Should().Throw<ValidationException>();
    }

    [TestMethod]
    public void WhenComparisonWithNonComparablePropertiesIsDone_InvalidOperationExceptionIsThrown()
    {
        var compareAttribute = new CompareToAttribute(ComparisonType.Equal, "OtherProperty");
        var ctx = new ValidationContext(new { OtherProperty = new { } });
        Action validate = () => compareAttribute.Validate(new { }, ctx);
        validate.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void WhenComparisonWithNullValuesIsDone_NoExceptionIsThrown()
    {
        var compareAttribute = new CompareToAttribute(ComparisonType.Equal, "OtherProperty");

        var ctx1 = new ValidationContext(new { OtherProperty = (int?)null });
        Action validate1 = () => compareAttribute.Validate(1, ctx1);

        var ctx2 = new ValidationContext(new { OtherProperty = 1 });
        Action validate2 = () => compareAttribute.Validate(null, ctx2);

        using (new AssertionScope())
        {
            validate1.Should().NotThrow();
            validate2.Should().NotThrow();
        }
    }

    public static IEnumerable<object[]> GetValidNumericalComparison()
    {
        yield return new object[] { ComparisonType.Equal, 1, 1 };
        yield return new object[] { ComparisonType.NotEqual, 1, 2 };
        yield return new object[] { ComparisonType.LessThan, 1, 2 };
        yield return new object[] { ComparisonType.LessThanOrEqual, 1, 1 };
        yield return new object[] { ComparisonType.LessThanOrEqual, 1, 2 };
        yield return new object[] { ComparisonType.GreaterThan, 2, 1 };
        yield return new object[] { ComparisonType.GreaterThanOrEqual, 1, 1 };
        yield return new object[] { ComparisonType.GreaterThanOrEqual, 2, 1 };
    }

    public static IEnumerable<object[]> GetInvalidNumericalComparison()
    {
        yield return new object[] { ComparisonType.Equal, 1, 2 };
        yield return new object[] { ComparisonType.NotEqual, 1, 1 };
        yield return new object[] { ComparisonType.LessThan, 1, 1 };
        yield return new object[] { ComparisonType.LessThan, 2, 1 };
        yield return new object[] { ComparisonType.LessThanOrEqual, 2, 1 };
        yield return new object[] { ComparisonType.GreaterThan, 1, 1 };
        yield return new object[] { ComparisonType.GreaterThan, 1, 2 };
        yield return new object[] { ComparisonType.GreaterThanOrEqual, 1, 2 };
    }

    private static readonly DateTime _dateTimeEarlier = new DateTime(2020, 01, 01);
    private static readonly DateTime _dateTimeLater = new DateTime(2020, 01, 02);
    public static IEnumerable<object[]> GetValidDateTimeComparison()
    {
        yield return new object[] { ComparisonType.Equal, _dateTimeEarlier, _dateTimeEarlier };
        yield return new object[] { ComparisonType.NotEqual, _dateTimeEarlier, _dateTimeLater };
        yield return new object[] { ComparisonType.LessThan, _dateTimeEarlier, _dateTimeLater };
        yield return new object[] { ComparisonType.LessThanOrEqual, _dateTimeEarlier, _dateTimeEarlier };
        yield return new object[] { ComparisonType.LessThanOrEqual, _dateTimeEarlier, _dateTimeLater };
        yield return new object[] { ComparisonType.GreaterThan, _dateTimeLater, _dateTimeEarlier };
        yield return new object[] { ComparisonType.GreaterThanOrEqual, _dateTimeEarlier, _dateTimeEarlier };
        yield return new object[] { ComparisonType.GreaterThanOrEqual, _dateTimeLater, _dateTimeEarlier };
    }

    public static IEnumerable<object[]> GetInvalidDateTimeComparison()
    {
        yield return new object[] { ComparisonType.Equal, _dateTimeEarlier, _dateTimeLater };
        yield return new object[] { ComparisonType.NotEqual, _dateTimeEarlier, _dateTimeEarlier };
        yield return new object[] { ComparisonType.LessThan, _dateTimeEarlier, _dateTimeEarlier };
        yield return new object[] { ComparisonType.LessThan, _dateTimeLater, _dateTimeEarlier };
        yield return new object[] { ComparisonType.LessThanOrEqual, _dateTimeLater, _dateTimeEarlier };
        yield return new object[] { ComparisonType.GreaterThan, _dateTimeEarlier, _dateTimeEarlier };
        yield return new object[] { ComparisonType.GreaterThan, _dateTimeEarlier, _dateTimeLater };
        yield return new object[] { ComparisonType.GreaterThanOrEqual, _dateTimeEarlier, _dateTimeLater };
    }
}