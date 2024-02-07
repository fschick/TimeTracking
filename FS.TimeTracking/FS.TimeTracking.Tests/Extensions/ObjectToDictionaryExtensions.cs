using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Tests.Extensions;

/// <summary>
/// Extension methods to flatten/un-flatten object to/from path/values pairs.
/// </summary>
public static class ObjectToDictionaryExtensions
{
    /// <summary>
    /// Converts an object to key/value pairs to be used with 'MemoryConfigurationBuilderExtensions.AddInMemoryCollection'.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    public static Dictionary<string, string> ToOptionsCollection(this object obj)
        => JObject.FromObject(obj)
            .Descendants()
            .OfType<JValue>()
            .ToDictionary(
                jValue => jValue.Path.Replace('.', ':'),
                jValue => jValue.Value as string ?? JsonConvert.SerializeObject(jValue.Value)
            );
}