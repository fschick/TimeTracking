using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Extensions;

/// <summary>
/// Extensions methods for type <see cref="IEnumerable{T}"></see>
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Asynchronously projects each element of an <see cref="IEnumerable{T}"/> into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <param name="source">A sequence of values to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="concurrency">Max tasks that will be executed in parallel.</param>
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> selector, int concurrency = int.MaxValue)
    {
        using var semaphore = new SemaphoreSlim(concurrency);
        return await Task.WhenAll(source.Select(async s =>
        {
            try
            {
                await semaphore.WaitAsync();
                return await selector(s);
            }
            finally
            {
                semaphore.Release();
            }
        }));
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="IEnumerable{T}"/> and flattens the resulting sequences into one sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <param name="source">A sequence of values to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="concurrency">Max tasks that will be executed in parallel.</param>
    public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<IEnumerable<TResult>>> selector, int concurrency = int.MaxValue)
        => (await SelectAsync(source, selector, concurrency)).SelectMany(x => x);
}