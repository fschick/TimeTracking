using Newtonsoft.Json;
using Plainquire.Filter.Newtonsoft;

namespace FS.TimeTracking.Application.Tests.Models;

public class TestCase
{
    public string Identifier { get; set; }

    public string ToJson()
        => JsonConvert.SerializeObject(this, _jsonSerializerSettings);

    public static T FromJson<T>(string json)
        => JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);

    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        Converters = JsonConverterExtensions.NewtonsoftConverters,
    };
}