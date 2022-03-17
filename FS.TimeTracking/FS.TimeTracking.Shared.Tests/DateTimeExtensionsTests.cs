using FluentAssertions;
using FS.TimeTracking.Abstractions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Shared.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class DateTimeExtensionsTests
{
    [DataTestMethod]
    [DynamicData(nameof(GetStartEndDateTimeRanges), DynamicDataSourceType.Method)]
    public void WhenDatesBetweenTwoDatesRequested_CountMatchesExpected(DateTime start, DateTime end, int expectedDayCount)
    {
        start.GetDays(end).Should().HaveCount(expectedDayCount);
    }

    public static IEnumerable<object[]> GetStartEndDateTimeRanges()
    {
        yield return new object[] { new DateTime(2000, 1, 1), new DateTime(2000, 1, 1), 1 };
        yield return new object[] { new DateTime(2000, 1, 1), new DateTime(2000, 1, 31), 31 };
    }
}