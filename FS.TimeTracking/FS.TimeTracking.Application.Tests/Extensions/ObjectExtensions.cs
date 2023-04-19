using Newtonsoft.Json;

namespace FS.TimeTracking.Application.Tests.Extensions;

public static class ObjectExtensions
{
    public static T JsonClone<T>(this T obj)
        => obj != null
            ? JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj))
            : default;
}