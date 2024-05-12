using Microsoft.Extensions.DependencyInjection;
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Mvc.Newtonsoft;
using Plainquire.Filter.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class PlainquireStartup
{
    public static IMvcBuilder AddPlainquire(this IMvcBuilder mvcBuilder)
        => mvcBuilder
            .AddFilterSupport()
            .AddFilterNewtonsoftSupport();

    public static SwaggerGenOptions AddPlainquire(this SwaggerGenOptions options, params string[] xmlDocumentationFilePaths)
        => options.AddFilterSupport(xmlDocumentationFilePaths);
}