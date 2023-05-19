using System;
using System.Security.Cryptography;
using System.Text;

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

    /// <summary>
    /// Computes the hash of string the SHA256 algorithm.
    /// </summary>
    /// <param name="value">The string to hash.</param>
    public static string HashSHA256(this string value)
    {
        var hash = SHA256.HashData(Encoding.Default.GetBytes(value));
        return Convert.ToHexString(hash).ToLower();
    }
}