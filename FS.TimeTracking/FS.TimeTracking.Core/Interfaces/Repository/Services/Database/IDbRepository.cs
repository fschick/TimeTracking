// The MIT License (MIT)
// Copyright (c) 2021 © Florian Schick, 2021 all rights reserved
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.

using FS.TimeTracking.Core.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FS.TimeTracking.Core.Interfaces.Repository.Services.Database;

/// <summary>
/// Generic repository.
/// </summary>
public interface IDbRepository
{
    /// <summary>
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
    /// Gets a projection of entities from database using <see cref="AutoMapper"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to return.</typeparam>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="orderBy">A function to order the result.</param>
    /// <param name="includes">The names of the navigation properties to include in result.</param>
    /// <param name="distinct">If set to <c>true</c> to return only distinct values.</param>
    /// <param name="skip">Bypasses a specified number of elements.</param>
    /// <param name="take">Returns a specified number of contiguous elements from the result.</param>
    /// <param name="tracked">if set to <c>true</c>, selected entities are tracked for later operations.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<TDto>> Get<TEntity, TDto>(
        Expression<Func<TEntity, bool>> where = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string[] includes = null,
        bool distinct = false,
        int? skip = null,
        int? take = null,
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
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
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
    /// Gets the first projection of entities from database using <see cref="AutoMapper"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to return.</typeparam>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="orderBy">A function to order the result.</param>
    /// <param name="includes">The names of the navigation properties to include in result.</param>
    /// <param name="skip">Bypasses a specified number of elements.</param>
    /// <param name="tracked">if set to <c>true</c>, selected entities are tracked for later operations.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TDto> FirstOrDefault<TEntity, TDto>(
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
    /// Computes the sum of the sequence of values that is obtained by invoking a projection function on  each element of the input sequence.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<long> Sum<TEntity>(
        Expression<Func<TEntity, long>> select,
        Expression<Func<TEntity, bool>> where = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Computes the min of the sequence of values that is obtained by invoking a projection function on  each element of the input sequence.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TResult> Min<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Computes the max of the sequence of values that is obtained by invoking a projection function on  each element of the input sequence.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="select">Projects each entity into desired result.</param>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TResult> Max<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Tests if entities exists.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="where">Filters the entities based on a predicate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<bool> Exists<TEntity>(
        Expression<Func<TEntity, bool>> where = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class;

    /// <summary>
    /// Adds the specified entity to database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TEntity> Add<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntityModel;

    /// <summary>
    /// Adds a range of specified entities to database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<List<TEntity>> AddRange<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntityModel;

    /// <summary>
    /// Adds a range of specified entities to database. Data is immediately written to the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<List<TEntity>> BulkAddRange<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntityModel;

    /// <summary>
    /// Updates the specified entity in database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns></returns>
    TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntityModel;

    /// <summary>
    /// Bulk update the specific entity in database. Data is immediately written to the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <param name="where">Predicate to filter the entities to update.</param>
    /// <param name="setter">A setter in the form of <code>previous => new TEntity { PropertyToSet = value }</code>.</param>
    /// <returns> Number of updated records.</returns>
    Task<int> BulkUpdate<TEntity>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TEntity>> setter) where TEntity : class;

    /// <summary>
    /// Removes the specified entity from database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to remove.</param>
    /// <returns></returns>
    TEntity Remove<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Removes a range of specified entities from database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to remove.</param>
    /// <returns></returns>
    List<TEntity> Remove<TEntity>(List<TEntity> entities) where TEntity : class;

    /// <summary>
    /// Removes a range of entities specified by a predicate. Data is immediately written to the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="where">Filters the entities based on a predicate.</param>
    Task<int> BulkRemove<TEntity>(Expression<Func<TEntity, bool>> where = null) where TEntity : class;

    /// <summary>
    /// Creates a new transaction scope. Transaction scopes can be nested.
    /// </summary>
    TransactionScope CreateTransactionScope();

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChanges(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all migrations that are defined in the assembly but haven't been applied to the target database.
    /// </summary>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task<IEnumerable<string>> GetPendingMigrations(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all migrations that have been applied to the target database.
    /// </summary>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<IEnumerable<string>> GetAppliedMigrations(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a hash of the current database model
    /// </summary>
    Task<string> GetDatabaseModelHash(CancellationToken cancellationToken = default);
}