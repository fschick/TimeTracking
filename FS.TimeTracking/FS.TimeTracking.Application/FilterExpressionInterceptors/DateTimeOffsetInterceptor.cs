using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using Plainquire.Filter;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.PropertyFilterExpression;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.TimeTracking.Application.FilterExpressionInterceptors;

internal class DateTimeOffsetInterceptor : IFilterInterceptor
{
    public Expression<Func<TEntity, bool>> CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, ValueFilter[] filters, FilterConfiguration configuration)
    {
        if (typeof(TEntity) == typeof(TimeSheet) && propertyInfo.Name == nameof(TimeSheet.StartDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpression.CreateFilter((TimeSheet x) => x.StartDateLocal, filters, configuration, this);

        if (typeof(TEntity) == typeof(TimeSheet) && propertyInfo.Name == nameof(TimeSheet.EndDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpression.CreateFilter((TimeSheet x) => x.EndDateLocal, filters, configuration, this);

        if (typeof(TEntity) == typeof(Order) && propertyInfo.Name == nameof(Order.StartDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpression.CreateFilter((Order x) => x.StartDateLocal, filters, configuration, this);

        if (typeof(TEntity) == typeof(Order) && propertyInfo.Name == nameof(Order.DueDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpression.CreateFilter((Order x) => x.DueDateLocal, filters, configuration, this);

        if (typeof(TEntity) == typeof(Holiday) && propertyInfo.Name == nameof(Holiday.StartDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpression.CreateFilter((Holiday x) => x.StartDateLocal, filters, configuration, this);

        if (typeof(TEntity) == typeof(Holiday) && propertyInfo.Name == nameof(Holiday.EndDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpression.CreateFilter((Holiday x) => x.EndDateLocal, filters, configuration, this);

        return null;

        // Doesn't work because 'DateTimeExtensions.ToUtc' does not become registered when used via Expression.Call(...)
        //var dateLocalSelector = typeof(TEntity).CreatePropertySelector<TEntity, DateTime>($"{propertyInfo.Name}Local");
        //var offsetSelector = typeof(TEntity).CreatePropertySelector<TEntity, int>($"{propertyInfo.Name}Offset");
        //var toUtcMethod = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.ToUtc), new[] { typeof(DateTime), typeof(int) });
        //var dateLocalToUtc = Expression.Call(toUtcMethod!, dateLocalSelector.Body, offsetSelector.Body);
        //var dateLocalToUtcLambda = dateLocalSelector.CreateLambda<TEntity, DateTime>(dateLocalToUtc);
        //var dateTimeFilterExpression = PropertyFilterExpressionCreator.CreateFilter(dateLocalToUtcLambda, filter, filterConfiguration);
        //return dateTimeFilterExpression;
    }

    public Func<DateTimeOffset> Now => () => DateTimeOffset.Now;
}