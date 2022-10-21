using FS.TimeTracking.Application.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Application.Tests.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public abstract class TestCaseDataSourceAttribute : Attribute, ITestDataSource
{
    private readonly List<TestCase> _testCases;

    protected TestCaseDataSourceAttribute(List<TestCase> testCases)
        => _testCases = EnsureUniqueTestCaseIdentifiers(testCases);

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => _testCases.Select(testCase => new object[] { testCase.ToJson() });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => $"{methodInfo.Name}_{TestCase.FromJson<TestCase>((string)data[0]).Identifier}";

    private static List<TestCase> EnsureUniqueTestCaseIdentifiers(List<TestCase> testCases)
    {
        var duplicateIdentifiers = testCases
            .GroupBy(x => x.Identifier)
            .Where(group => group.Count() > 1)
            .Select(x => x.Key)
            .Distinct()
            .ToList();

        return duplicateIdentifiers.Any()
            ? NonUniqueTestCaseIdentifier.Create($"All_test_cases_requires_an_unique_identifier_Duplicates_Identifiers_{string.Join("_", duplicateIdentifiers)}")
            : testCases;
    }

    private class NonUniqueTestCaseIdentifier : TestCase
    {
        private NonUniqueTestCaseIdentifier() { }

        public static List<TestCase> Create(string errorMessage)
            => new() { new NonUniqueTestCaseIdentifier { Identifier = errorMessage } };
    }
}