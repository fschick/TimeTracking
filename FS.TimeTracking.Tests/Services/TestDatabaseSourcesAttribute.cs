using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Tests.Services
{
    public class TestDatabaseSourcesAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            var testDatabaseSourcesFile = Environment.GetEnvironmentVariable("TestDatabaseSources");
            testDatabaseSourcesFile ??= "TestDatabaseSources.json";
            var testDatabaseSourcesJson = File.ReadAllText(testDatabaseSourcesFile);
            var testDatabaseSources = JsonConvert.DeserializeObject<List<DatabaseConfiguration>>(testDatabaseSourcesJson);
            return testDatabaseSources.Select(x => new object[] { x });
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var databaseType = ((DatabaseConfiguration)data[0]).Type;
            return $"{methodInfo.Name}_{databaseType}";
        }
    }
}
