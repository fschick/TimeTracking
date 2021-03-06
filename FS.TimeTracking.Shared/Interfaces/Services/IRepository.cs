﻿using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Services
{
    /// <summary>
    /// Generic repository.
    /// </summary>
    public interface IRepository
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
            ) where TEntity : class, IEntityModel;

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
            ) where TEntity : class, IEntityModel;

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
            ) where TEntity : class, IEntityModel;

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
            ) where TEntity : class, IEntityModel;

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
            ) where TEntity : class, IEntityModel;

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
        /// Adds a range of specified entities to database.
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
        /// Removes the specified entity from database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to remove.</param>
        /// <returns></returns>
        TEntity Remove<TEntity>(TEntity entity) where TEntity : class, IEntityModel;

        /// <summary>
        /// Removes a range of specified entities from database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The entities to remove.</param>
        /// <returns></returns>
        List<TEntity> Remove<TEntity>(List<TEntity> entities) where TEntity : class, IEntityModel;

        /// <summary>
        /// Removes a range of entities specified by a predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="where">Filters the entities based on a predicate.</param>
        Task<int> Remove<TEntity>(Expression<Func<TEntity, bool>> where = null) where TEntity : class, IEntityModel;

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}
