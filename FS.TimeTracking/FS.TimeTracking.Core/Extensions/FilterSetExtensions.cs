using FS.TimeTracking.Core.Constants;
using FS.TimeTracking.Core.Models.Filter;
using Plainquire.Filter;
using Plainquire.Filter.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FS.TimeTracking.Core.Extensions;

/// <summary>
/// Extensions for <see cref="TimeSheetFilterSet"/>
/// </summary>
public static class FilterSetExtensions
{
    /// <summary>
    /// Gets the selection period from <paramref name="filters.TimeSheetFilter"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    /// <param name="endDateExclusive">Handle given end date as (right) exclusive.</param>
    public static Task<Range<DateTimeOffset>> GetSelectedPeriod(this TimeSheetFilterSet filters, bool endDateExclusive = false)
    {
        var startDateFilter = filters.TimeSheetFilter.GetPropertyFilterSyntax(x => x.StartDate);
        var endDateFilter = filters.TimeSheetFilter.GetPropertyFilterSyntax(x => x.EndDate);

        var startDate = endDateFilter != null
            ? ValueFiltersFactory.Create(endDateFilter).First().Value!.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : DateOffset.MinDate;

        var endDate = startDateFilter != null
            ? ValueFilter.Create(startDateFilter).Value!.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : DateOffset.MaxDate; // Let space for timezone conversions.

        if (endDate != DateOffset.MaxDate && endDateExclusive)
            endDate = endDate.AddDays(-1);

        return Task.FromResult(new Range<DateTimeOffset>(startDate, endDate));
    }

    /// <summary>
    /// Creates HTTP query parameters from given filters.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    /// <param name="additionalParameters">A variable-length parameters list containing additional parameters.</param>
    public static Task<string> ToQueryParams(this TimeSheetFilterSet filters, params (string key, string value)[] additionalParameters)
    {
        var filterParameters = new[]
        {
            filters.TimeSheetFilter.ToQueryParams(),
            filters.ProjectFilter.ToQueryParams(),
            filters.CustomerFilter.ToQueryParams(),
            filters.ActivityFilter.ToQueryParams(),
            filters.OrderFilter.ToQueryParams(),
            filters.HolidayFilter.ToQueryParams(),
        };

        var additionalParams = additionalParameters.Select(param => $"{HttpUtility.UrlEncode(param.key)}={HttpUtility.UrlEncode(param.value)}");
        var keyValuePairs = filterParameters.Concat(additionalParams).Where(x => !string.IsNullOrWhiteSpace(x));
        return Task.FromResult(string.Join('&', keyValuePairs));
    }
}