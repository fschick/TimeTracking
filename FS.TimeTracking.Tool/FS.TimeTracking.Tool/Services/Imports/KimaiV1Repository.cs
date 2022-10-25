using FS.TimeTracking.Tool.DbContexts.Imports;
using FS.TimeTracking.Tool.Interfaces.Import;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FS.TimeTracking.Tool.Services.Imports;

/// <inheritdoc cref="Repository{TDbContext}" />
public class KimaiV1Repository : IKimaiV1Repository
{
    private readonly KimaiV1DbContext _dbContext;

    public KimaiV1Repository(KimaiV1DbContext dbContext)
        => _dbContext = dbContext;

    /// <inheritdoc />
    public Task<List<TResult>> Get<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string[] includes = null,
        bool distinct = false,
        int? skip = null,
        int? take = null,
        bool tracked = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class
        => GetInternal(x => x.Select(select), where, orderBy, null, includes, distinct, skip, take, tracked)
            .ToListAsyncEF(cancellationToken);

    /// <inheritdoc />
    public Task<List<TResult>> GetGrouped<TEntity, TGroupByKey, TResult>(
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
    ) where TEntity : class
        => GetInternal(x => x.GroupBy(groupBy).Select(select), where, null, orderBy, includes, distinct, skip, take, tracked)
            .ToListAsyncEF(cancellationToken);

    /// <inheritdoc />
    public Task<TResult> FirstOrDefault<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string[] includes = null,
        int? skip = null,
        bool tracked = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class
        => GetInternal(x => x.Select(select), where, orderBy, null, includes, false, skip, null, tracked)
            .FirstOrDefaultAsyncEF(cancellationToken);

    /// <inheritdoc />
    public Task<long> Count<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        bool distinct = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class
        => GetInternal(x => x.Select(select), where, null, null, null, distinct, null, null, false)
            .LongCountAsyncEF(cancellationToken);

    /// <inheritdoc />
    public Task<bool> Exists<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class
        => GetInternal(x => x.Select(select), where, null, null, null, false, null, null, false)
            .AnyAsyncEF(cancellationToken);

    /// <inheritdoc />
    public TransactionScope CreateTransactionScope()
        => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    private IQueryable<TResult> GetInternal<TEntity, TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> select,
        Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> entityOrderBy,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>> resultOrderBy,
        string[] includes,
        bool distinct,
        int? skip,
        int? take,
        bool tracked
    ) where TEntity : class
    {
        var query = _dbContext
            .Set<TEntity>()
            .AsQueryable();

        if (!tracked)
            query = query.AsNoTracking();

        if (where != null)
            query = query.Where(where);

        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);

        if (entityOrderBy != null)
            query = entityOrderBy(query);

        var result = select(query);

        if (resultOrderBy != null)
            result = resultOrderBy(result);

        if (distinct)
            result = result.Distinct();

        if (skip != null)
            result = result.Skip(skip.Value);

        if (take != null)
            result = result.Take(take.Value);

        return result;
    }
}