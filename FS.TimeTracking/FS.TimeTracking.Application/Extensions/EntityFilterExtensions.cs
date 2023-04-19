using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FS.TimeTracking.Application.Extensions;

internal static class EntityFilterExtensions
{
    public static EntityFilter<TEntity> SetHidden<TEntity>(this EntityFilter<TEntity> filter, Expression<Func<TEntity, bool>> hiddenSelector, bool showHidden)
    {
        if (!showHidden)
            filter.Replace(hiddenSelector, false);
        return filter;
    }

    public static EntityFilter<TEntity> RestrictToCustomers<TEntity>(this EntityFilter<TEntity> filter, Expression<Func<TEntity, Guid?>> customerIdSelector, IEnumerable<Guid> customerIds)
    {
        var allowedIds = customerIds.EmptyIfNull().ToArray();
        if (allowedIds.Any())
            filter.Add(customerIdSelector, allowedIds);
        return filter;
    }
}