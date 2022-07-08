using FluentAssertions;
using FS.TimeTracking.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class DateTimeExtensionsTests
{
    [DataTestMethod]
    [DynamicData(nameof(WhenDaysBetweenTwoDatesRequested_CountMatchesExpectedDays_Data), DynamicDataSourceType.Method)]
    public void WhenDaysBetweenTwoDatesRequested_CountMatchesExpectedDays(DateTime start, DateTime end, int expectedDayCount)
        => start.GetDays(end).Should().HaveCount(expectedDayCount);

    public static IEnumerable<object[]> WhenDaysBetweenTwoDatesRequested_CountMatchesExpectedDays_Data()
    {
        yield return new object[] { new DateTime(2000, 1, 1, 12, 0, 0), new DateTime(2000, 1, 1), 1 };
        yield return new object[] { new DateTime(2000, 1, 1, 12, 0, 0), new DateTime(2000, 1, 31), 31 };
    }

    [DataTestMethod]
    [DynamicData(nameof(WhenStartOfYearIsRequested_StartOfYearIsReturned_Data), DynamicDataSourceType.Method)]
    public void WhenStartOfYearIsRequested_StartOfYearIsReturned(DateTime dateTime, DateTime expected)
        => dateTime.StartOfYear().Should().Be(expected);

    public static IEnumerable<object[]> WhenStartOfYearIsRequested_StartOfYearIsReturned_Data()
    {
        yield return new object[] { new DateTime(2000, 1, 1, 12, 0, 0), new DateTime(2000, 1, 1) };
        yield return new object[] { new DateTime(2000, 3, 31, 12, 0, 0), new DateTime(2000, 1, 1) };
        yield return new object[] { new DateTime(2000, 12, 31, 12, 0, 0), new DateTime(2000, 1, 1) };
    }

    [DataTestMethod]
    [DynamicData(nameof(WhenStartOfMonthIsRequested_StartOfYearIsReturned_Data), DynamicDataSourceType.Method)]
    public void WhenStartOfMonthIsRequested_StartOfYearIsReturned(DateTime dateTime, DateTime expected)
        => dateTime.StartOfMonth().Should().Be(expected);

    public static IEnumerable<object[]> WhenStartOfMonthIsRequested_StartOfYearIsReturned_Data()
    {
        yield return new object[] { new DateTime(2000, 1, 1, 12, 0, 0), new DateTime(2000, 1, 1) };
        yield return new object[] { new DateTime(2000, 3, 31, 12, 0, 0), new DateTime(2000, 3, 1) };
        yield return new object[] { new DateTime(2000, 12, 31, 12, 0, 0), new DateTime(2000, 12, 1) };
    }

    [DataTestMethod]
    [DynamicData(nameof(WhenStartOfWeekIsRequested_StartOfYearIsReturned_Data), DynamicDataSourceType.Method)]
    public void WhenStartOfWeekIsRequested_StartOfYearIsReturned(DateTime dateTime, DateTime expected)
        => dateTime.StartOfWeek().Should().Be(expected);

    public static IEnumerable<object[]> WhenStartOfWeekIsRequested_StartOfYearIsReturned_Data()
    {
        yield return new object[] { new DateTime(2000, 1, 1, 12, 0, 0), new DateTime(1999, 12, 27) };
        yield return new object[] { new DateTime(2000, 1, 9, 12, 0, 0), new DateTime(2000, 1, 3) };
        yield return new object[] { new DateTime(2000, 1, 10, 12, 0, 0), new DateTime(2000, 1, 10) };
    }
}