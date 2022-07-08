using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Tests.Services;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestDatabasesAttribute : TestCategoryBaseAttribute, ITestDataSource
{
    public override IList<string> TestCategories => new List<string> { "DatabaseRequired" };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var testDatabasesFile = Environment.GetEnvironmentVariable("TestDatabases");
        testDatabasesFile ??= "TestDatabases.json";
        var testDatabaseSourcesJson = File.ReadAllText(testDatabasesFile);
        var testDatabaseSources = JsonConvert.DeserializeObject<List<DatabaseConfiguration>>(testDatabaseSourcesJson);
        return testDatabaseSources!.Select(x => new object[] { x });
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    {
        var databaseType = ((DatabaseConfiguration)data[0]).Type;
        return $"{methodInfo.Name}_{databaseType}";
    }
}