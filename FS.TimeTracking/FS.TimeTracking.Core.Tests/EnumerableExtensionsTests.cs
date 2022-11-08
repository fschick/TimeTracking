using FluentAssertions;
using FS.TimeTracking.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Core.Tests;

[TestClass]
public class EnumerableExtensionsTests
{
    [TestMethod]
    public void WhenOuterJoinWithSmallerEnumerableIsApplied_ResultMatchesExpected()
    {
        var inner = new List<TestItemInner> { new() { Id = "I1", InnerKey = 1 }, new() { Id = "I2", InnerKey = 2 } }.AsEnumerable();
        var outer = new List<TestItemOuter> { new() { Id = "O1", OuterKey = 1 } }.AsEnumerable();

        var joined = inner.OuterJoin(outer, static x => x.InnerKey, x => x.OuterKey, (i, o) => new { Inner = i, Outer = o }).ToList();
        joined.Should().HaveCount(2);
        joined.Last().Outer.Should().BeNull();
    }

    [TestMethod]
    public void WhenOuterJoinWithLargerEnumerableIsApplied_ResultMatchesExpected()
    {
        var inner = new List<TestItemInner> { new() { Id = "I1", InnerKey = 1 } }.AsEnumerable();
        var outer = new List<TestItemOuter> { new() { Id = "O1", OuterKey = 1 }, new() { Id = "O2", OuterKey = 2 } }.AsEnumerable();

        var joined = inner.OuterJoin(outer, static x => x.InnerKey, x => x.OuterKey, (i, o) => new { Inner = i, Outer = o }).ToList();
        joined.Should().HaveCount(1);
        joined.Last().Outer.Should().NotBeNull();
    }



    [TestMethod]
    public void WhenCrossJoinIsApplied_ResultMatchesExpected()
    {
        var inner = new List<TestItemInner> { new() { Id = "I1", InnerKey = 1 }, new() { Id = "I2", InnerKey = 2 } }.AsEnumerable();
        var outer = new List<TestItemOuter> { new() { Id = "O2", OuterKey = 2 }, new() { Id = "O3", OuterKey = 3 } }.AsEnumerable();

        var joined = inner.CrossJoin(outer, static x => x.InnerKey, x => x.OuterKey, (i, o) => new { Inner = i, Outer = o }).ToList();
        joined.Should().HaveCount(3);
        joined.First().Outer.Should().BeNull();
        joined.Last().Inner.Should().BeNull();
    }

    private class TestItemInner
    {
        public string Id { get; set; }
        public int InnerKey { get; set; }
    }

    private class TestItemOuter
    {
        public string Id { get; set; }
        public int OuterKey { get; set; }
    }
}