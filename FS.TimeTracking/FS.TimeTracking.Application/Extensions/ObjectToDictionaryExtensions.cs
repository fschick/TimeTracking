using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Application.Extensions;

/// <summary>
/// Extension methods to flatten/un-flatten object to/from path/values pairs.
/// </summary>
public static class ObjectToDictionaryExtensions
{
    /// <summary>
    /// Converts an object to path/value pairs.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    public static Dictionary<string, string> ToDictionary(this object obj)
        => JObject.FromObject(obj)
            .Descendants()
            .OfType<JValue>()
            .ToDictionary(
                jValue => jValue.Path,
                jValue => JsonConvert.SerializeObject(jValue.Value)
            );

    /// <summary>
    /// Creates an object from path/value pairs.
    /// </summary>
    /// <param name="dictionary">A dictionary holding the path/value pairs to create the object.</param>
    public static T ToObject<T>(this Dictionary<string, string> dictionary)
    {
        var jsonLines = dictionary.Select(x => $"\"{x.Key}\": {x.Value}");
        var json = $"{{ {string.Join("\r\n,", jsonLines)} }}";

        var flatObject = JObject.Parse(json);
        var complexObject = new JObject();
        foreach (var (key, value) in flatObject)
            SetPropertyFromPath(complexObject, key, value);

        return complexObject.ToObject<T>();
    }

    private static void SetPropertyFromPath(JObject obj, string path, JToken value)
    {
        var segments = path.Split('.');
        var pathSegments = segments[..^1];
        var propertyName = segments[^1];

        var child = obj;
        foreach (var segment in pathSegments)
        {
            if (!child.ContainsKey(segment) /* || child[segment] is JValue { Value: null }*/)
                child[segment] = new JObject();
            child = child[segment] as JObject;
            if (child == null)
                throw new InvalidOperationException($"The dictionary contains more than one element matching parts of the path {path}");
        }

        child[propertyName] = value;
    }
}