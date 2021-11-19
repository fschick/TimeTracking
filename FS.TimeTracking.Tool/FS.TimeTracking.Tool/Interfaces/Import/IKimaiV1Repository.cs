using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FS.TimeTracking.Tool.Interfaces.Import;

public interface IKimaiV1Repository
{/// <summary>
 /// Gets a projection of entities from database.
 /// </summary>
 /// <typeparam name="TEntity">The type of the entity.</typeparam>
 /// <typeparam name="TResult">The type of the result.</typeparam>
 /// <param name="select">Projects each entity into desired result.</param>
 /// <param name="where">Filters the entities based on a predicate.</param>
 /// <param name="orderBy">A function to order the result.</param>
 /// <param name="includes">The names of the navigation properties to include in result.</param>
 /// <param name="distinct">If set to <c>true</c> to return only distinct values.</param>
 /// <param name="skip">Bypasses a specified number of elements.</param>
 /// <param name="take">Returns a specified number of contiguous elements from the result.</param>
 /// <param name="tracked">if set to <c>true</c>, selected entities are tracked for later operations.</param>
 /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
 /// <returns></returns>
    Task<List<TResult>> Get<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string[] includes = null, bool distinct = false,
        int? skip = null, int? take = null,
        bool tracked = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Gets a projection of entities from database grouped by given key(s).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TGroupByKey">The type of the group by key.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="groupBy">Groups the entities by given key.</param>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="orderBy">A function to order the result.</param>
    /// <param name="includes">The names of the navigation properties to include in result.</param>
    /// <param name="distinct">If set to <c>true</c> to return only distinct values.</param>
    /// <param name="skip">Bypasses a specified number of elements.</param>
    /// <param name="take">Returns a specified number of contiguous elements from the result.</param>
    /// <param name="tracked">if set to <c>true</c>, selected entities are tracked for later operations.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<List<TResult>> GetGrouped<TEntity, TGroupByKey, TResult>(
        Expression<Func<TEntity, TGroupByKey>> groupBy,
        Expression<Func<IGrouping<TGroupByKey, TEntity>, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
        string[] includes = null,
        bool distinct = false,
        int? skip = null,
        int? take = null,
        bool tracked = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Gets the first projection of entities from database.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="orderBy">A function to order the result.</param>
    /// <param name="includes">The names of the navigation properties to include in result.</param>
    /// <param name="skip">Bypasses a specified number of elements.</param>
    /// <param name="tracked">if set to <c>true</c>, selected entities are tracked for later operations.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TResult> FirstOrDefault<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string[] includes = null,
        int? skip = null,
        bool tracked = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Counts a projection of entities from database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="distinct">If set to <c>true</c> to count only distinct values.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<long> Count<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        bool distinct = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Test, if a projection of entities from database exists.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<bool> Exists<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Creates a new transaction scope. Transaction scopes can be nested.
    /// </summary>
    TransactionScope CreateTransactionScope();
}