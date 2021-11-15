using FS.FilterExpressionCreator.Mvc.Extensions;
using FS.FilterExpressionCreator.Mvc.Newtonsoft.Extensions;
using FS.FilterExpressionCreator.Swashbuckle.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class FilterExpressionCreator
{
    public static IMvcBuilder AddFilterExpressionCreators(this IMvcBuilder mvcBuilder)
        => mvcBuilder
            .AddFilterExpressionsSupport()
            .AddFilterExpressionsNewtonsoftSupport();

    public static SwaggerGenOptions AddFilterExpressionCreators(this SwaggerGenOptions options, params string[] xmlDocumentationFilePaths)
        => options.AddFilterExpressionsSupport(xmlDocumentationFilePaths);
}