using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Shared.Extensions
{
    /// <summary>
    /// Extension methods to convert NULL-References to an empty instance/array/enumerable.
    /// </summary>
    public static class EmptyIfNullExtensions
    {
        /// <summary>
        /// Returns a new instance of <typeparamref name="T"/>, if the given <paramref name="obj"/> is NULL. Otherwise <paramref name="obj"/> is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to test.</param>
        public static T EmptyIfNull<T>(this T obj) where T : new()
            => obj ?? new T();

        /// <summary>
        /// Returns an empty array, if the given <paramref name="obj"/> is NULL. Otherwise <paramref name="obj"/> is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to test.</param>
        public static T[] EmptyIfNull<T>(this T[] obj)
            => obj ?? new T[0];

        /// <summary>
        /// Returns an empty enumerable, if the given <paramref name="obj"/> is NULL. Otherwise <paramref name="obj"/> is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to test.</param>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> obj)
            => obj ?? Enumerable.Empty<T>();
    }
}
