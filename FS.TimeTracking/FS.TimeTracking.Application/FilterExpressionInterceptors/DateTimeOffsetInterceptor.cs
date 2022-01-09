using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.Models;
using FS.FilterExpressionCreator.PropertyFilterExpressionCreators;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using FS.TimeTracking.Shared.Models.Application.MasterData;
using FS.TimeTracking.Shared.Models.Application.TimeTracking;

namespace FS.TimeTracking.Application.FilterExpressionInterceptors;

internal class DateTimeOffsetInterceptor : IPropertyFilterInterceptor
{
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public Expression<Func<TEntity, bool>> CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, ValueFilter[] filters, FilterConfiguration filterConfiguration)
    {
        if (typeof(TEntity) == typeof(TimeSheet) && propertyInfo.Name == nameof(TimeSheet.StartDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((TimeSheet x) => x.StartDateLocal, filters, filterConfiguration);

        if (typeof(TEntity) == typeof(TimeSheet) && propertyInfo.Name == nameof(TimeSheet.EndDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((TimeSheet x) => x.EndDateLocal, filters, filterConfiguration);

        if (typeof(TEntity) == typeof(Order) && propertyInfo.Name == nameof(Order.StartDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((Order x) => x.StartDateLocal, filters, filterConfiguration);

        if (typeof(TEntity) == typeof(Order) && propertyInfo.Name == nameof(Order.DueDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((Order x) => x.DueDateLocal, filters, filterConfiguration);

        if (typeof(TEntity) == typeof(Holiday) && propertyInfo.Name == nameof(Holiday.StartDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((Holiday x) => x.StartDateLocal, filters, filterConfiguration);

        if (typeof(TEntity) == typeof(Holiday) && propertyInfo.Name == nameof(Holiday.EndDate))
            return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((Holiday x) => x.EndDateLocal, filters, filterConfiguration);

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
}