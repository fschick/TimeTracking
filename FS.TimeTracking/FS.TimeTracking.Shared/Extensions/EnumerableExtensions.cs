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

    /// <summary>
    /// Returns the input typed as <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    public static async Task<IEnumerable<T>> AsEnumerableAsync<T>(this Task<List<T>> source)
        => (await source).AsEnumerable();

    /// <summary>
    /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> source)
        => (await source).ToList();

    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    /// <param name="source">The source.</param>
    public static async Task<T> FirstAsync<T>(this Task<IEnumerable<T>> source)
        => (await source).First();

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequenz contains no elements.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
    public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source, T defaultValue = default)
        => (await source).FirstOrDefault(defaultValue);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="outer">The first sequence to join.</param>
    /// <param name="inner">The sequence to join to the first sequence.</param>
    /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
    /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
    /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
    public static IEnumerable<TResult> OuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        => outer
            .GroupJoin(inner, outerKeySelector, innerKeySelector, (o, i) => new { Outer = o, Inner = i })
            .SelectMany(join => join.Inner.DefaultIfEmpty(), (join, i) => resultSelector(join.Outer, i));

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="outer">The first sequence to join.</param>
    /// <param name="inner">The sequence to join to the first sequence.</param>
    /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
    /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
    /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
    public static IEnumerable<TResult> CrossJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
    {
        var outerList = new Lazy<IEnumerable<TOuter>>(() => outer as List<TOuter> ?? outer.ToList());
        var innerList = new Lazy<IEnumerable<TInner>>(() => inner as List<TInner> ?? inner.ToList());

        return outer
           .Select(outerKeySelector)
           .Union(innerList.Value.Select(innerKeySelector))
           .Select(key =>
           {
               var o = outerList.Value.FirstOrDefault(x => outerKeySelector(x).Equals(key));
               var i = innerList.Value.FirstOrDefault(x => innerKeySelector(x).Equals(key));
               return resultSelector(o, i);
           });
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="TimeSpan"/> values.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of <see cref="TimeSpan"/> values to calculate the sum of.</param>
    /// <param name="selector">The sum of the values in the sequence.</param>
    /// <returns>The sum of the projected values.</returns>
    public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> selector)
        => TimeSpan.FromTicks(source.Sum(x => selector(x).Ticks));
}