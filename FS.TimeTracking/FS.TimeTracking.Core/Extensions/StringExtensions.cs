namespace FS.TimeTracking.Core.Extensions;

/// <summary>
/// Extensions methods for type <see cref="string"></see>
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Lower cases the first character.
    /// </summary>
    /// <param name="value">The value.</param>
    public static string LowercaseFirstChar(this string value)
        => char.ToLower(value[0]) + value[1..];
}