using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.AutoMapper;
using FS.TimeTracking.Application.FilterExpressionInterceptors;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Application.Startup;

internal static class FilterExpressionInterceptor
{
    public static IServiceCollection RegisterFilterExpressionInterceptor(this IServiceCollection services)
    {
        EntityFilter.DefaultInterceptor = new DateTimeOffsetInterceptor();
        return services;
    }
}