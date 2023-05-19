using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Extensions;

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
    /// Asynchronously projects each element of an <see cref="IEnumerable{T}"/> into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <param name="source">A sequence of values to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this Task<IEnumerable<TSource>> source, Func<TSource, TResult> selector)
        => (await source).Select(selector);

    /// <inheritdoc cref="SelectAsync{TSource, TResult}(Task{IEnumerable{TSource}}, Func{TSource, TResult})" />
    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this Task<List<TSource>> source, Func<TSource, TResult> selector)
        => (await source).Select(selector);

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
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    public static async Task<IEnumerable<T>> WhereAsync<T>(this Task<IEnumerable<T>> source, Func<T, bool> predicate)
        => (await source).Where(predicate);

    /// <inheritdoc cref="WhereAsync{T}(Task{IEnumerable{T}}, Func{T, bool})" />
    public static async Task<IEnumerable<T>> WhereAsync<T>(this Task<List<T>> source, Func<T, bool> predicate)
        => (await source).Where(predicate);

    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    public static async Task<T> FirstAsync<T>(this Task<IEnumerable<T>> source, Func<T, bool> predicate = null)
        => predicate != null ? (await source).First(predicate) : (await source).First();

    /// <inheritdoc cref="FirstAsync{T}(Task{IEnumerable{T}}, Func{T, bool})" />
    public static async Task<T> FirstAsync<T>(this Task<List<T>> source, Func<T, bool> predicate = null)
        => predicate != null ? (await source).First(predicate) : (await source).First();

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
    public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source, T defaultValue = default)
        => (await source).FirstOrDefault(defaultValue);

    /// <inheritdoc cref="FirstOrDefaultAsync{T}(Task{IEnumerable{T}}, T)" />
    public static async Task<T> FirstOrDefaultAsync<T>(this Task<List<T>> source, T defaultValue = default)
        => (await source).FirstOrDefault(defaultValue);

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequenz contains no elements.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
    public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source, Func<T, bool> predicate, T defaultValue = default)
        => (await source).FirstOrDefault(predicate, defaultValue);

    /// <inheritdoc cref="FirstOrDefaultAsync{T}(Task{IEnumerable{T}}, Func{T, bool}, T)" />
    public static async Task<T> FirstOrDefaultAsync<T>(this Task<List<T>> source, Func<T, bool> predicate, T defaultValue = default)
        => (await source).FirstOrDefault(predicate, defaultValue);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by the function that is represented by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    public static async Task<IOrderedEnumerable<TSource>> OrderByAsync<TSource, TKey>(this Task<IEnumerable<TSource>> source, Func<TSource, TKey> keySelector)
        => (await source).OrderBy(keySelector);

    /// <summary>
    /// Produces the set difference of two sequences according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{TSource}" /> whose keys that are not also in <paramref name="second"/> will be returned.</param>
    /// <param name="second">An <see cref="IEnumerable{TKey}" /> whose keys that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
    {
        var set = new HashSet<TKey>(second.Select(keySelector));

        foreach (var element in first)
            if (set.Add(keySelector(element)))
                yield return element;
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

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    /// <typeparam name="TInner">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TOuter">The type of the elements of the second sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="inner">The first sequence to join.</param>
    /// <param name="outer">The sequence to join to the first sequence.</param>
    /// <param name="innerKeySelector">A function to extract the join key from each element of the first sequence.</param>
    /// <param name="outerKeySelector">A function to extract the join key from each element of the second sequence.</param>
    /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
    public static IEnumerable<TResult> OuterJoin<TInner, TOuter, TKey, TResult>(this IEnumerable<TInner> inner, IEnumerable<TOuter> outer, Func<TInner, TKey> innerKeySelector, Func<TOuter, TKey> outerKeySelector, Func<TInner, TOuter, TResult> resultSelector)
        => inner
            .GroupJoin(outer, innerKeySelector, outerKeySelector, (o, i) => new { Outer = o, Inner = i })
            .SelectMany(join => join.Inner.DefaultIfEmpty(), (join, i) => resultSelector(join.Outer, i));

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    /// <typeparam name="TInner">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TOuter">The type of the elements of the second sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="inner">The first sequence to join.</param>
    /// <param name="outer">The sequence to join to the first sequence.</param>
    /// <param name="innerKeySelector">A function to extract the join key from each element of the first sequence.</param>
    /// <param name="outerKeySelector">A function to extract the join key from each element of the second sequence.</param>
    /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
    public static IEnumerable<TResult> CrossJoin<TInner, TOuter, TKey, TResult>(this IEnumerable<TInner> inner, IEnumerable<TOuter> outer, Func<TInner, TKey> innerKeySelector, Func<TOuter, TKey> outerKeySelector, Func<TInner, TOuter, TResult> resultSelector)
    {
        var innerList = new Lazy<IEnumerable<TInner>>(() => inner as List<TInner> ?? inner.ToList());
        var outerList = new Lazy<IEnumerable<TOuter>>(() => outer as List<TOuter> ?? outer.ToList());

        return inner
           .Select(innerKeySelector)
           .Union(outerList.Value.Select(outerKeySelector))
           .Select(key =>
           {
               var i = innerList.Value.FirstOrDefault(x => innerKeySelector(x).Equals(key));
               var o = outerList.Value.FirstOrDefault(x => outerKeySelector(x).Equals(key));
               return resultSelector(i, o);
           });
    }

    /// <summary>
    /// Merges two sequences based on a <paramref name="keySelector"/>. Elements from the sequence <paramref name="merge"/> have precedence over <paramref name="origin"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <param name="origin">The origin sequence.</param>
    /// <param name="merge">The sequence to merge.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>
    /// The merged sequence.
    /// </returns>
    public static IEnumerable<TEntity> Merge<TEntity, TKey>(this IEnumerable<TEntity> origin, IEnumerable<TEntity> merge, Func<TEntity, TKey> keySelector)
    {
        var originList = new Lazy<IEnumerable<TEntity>>(() => origin as List<TEntity> ?? origin.ToList());
        var mergeList = new Lazy<IEnumerable<TEntity>>(() => merge as List<TEntity> ?? merge.ToList());

        return origin
            .Select(keySelector)
            .Union(mergeList.Value.Select(keySelector))
            .Select(key =>
            {
                if (mergeList.Value.Any(x => keySelector(x).Equals(key)))
                    return mergeList.Value.First(x => keySelector(x).Equals(key));
                return originList.Value.FirstOrDefault(x => keySelector(x).Equals(key));
            });
    }
}