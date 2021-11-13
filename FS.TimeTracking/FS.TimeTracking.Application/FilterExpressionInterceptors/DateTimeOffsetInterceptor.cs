using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.Models;
using FS.FilterExpressionCreator.PropertyFilterExpressionCreators;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Models.MasterData;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.FilterExpressionInterceptors
{
    internal class DateTimeOffsetInterceptor : IPropertyFilterInterceptor
    {
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public Expression<Func<TEntity, bool>> CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, ValueFilter filter, FilterConfiguration filterConfiguration)
        {
            if (typeof(TEntity) == typeof(TimeSheet) && propertyInfo.Name == nameof(TimeSheet.StartDate))
                return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((TimeSheet x) => x.StartDateLocal.ToUtc(x.StartDateOffset), filter, filterConfiguration);

            if (typeof(TEntity) == typeof(TimeSheet) && propertyInfo.Name == nameof(TimeSheet.EndDate))
                return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((TimeSheet x) => x.EndDateLocal.ToUtc(x.EndDateOffset.Value), filter, filterConfiguration);

            if (typeof(TEntity) == typeof(Order) && propertyInfo.Name == nameof(Order.StartDate))
                return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((Order x) => x.StartDateLocal.ToUtc(x.StartDateOffset), filter, filterConfiguration);

            if (typeof(TEntity) == typeof(Order) && propertyInfo.Name == nameof(Order.DueDate))
                return (Expression<Func<TEntity, bool>>)(object)PropertyFilterExpressionCreator.CreateFilter((Order x) => x.DueDateLocal.ToUtc(x.DueDateOffset), filter, filterConfiguration);
            
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
}
