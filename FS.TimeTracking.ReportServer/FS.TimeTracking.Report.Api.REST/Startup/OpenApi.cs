using FS.TimeTracking.Report.Api.REST.Filters;
using FS.TimeTracking.Report.Api.REST.Routing;
using FS.TimeTracking.Report.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace FS.TimeTracking.Report.Api.REST.Startup;

internal static class OpenApi
{
    public const string OPEN_API_UI_ROUTE = "openapi/report/";
    public const string OPEN_API_SPEC = "openapi.json";
    public const string SWAGGER_UI_ROUTE = "swagger/";

    internal static IServiceCollection RegisterOpenApiController(this IServiceCollection services)
        => services
            .AddSwaggerGenNewtonsoftSupport()
            .AddSwaggerGen(c =>
            {
                const string documentName = V1ApiController.API_VERSION;
                c.SwaggerDoc(documentName, new OpenApiInfo { Title = $"{AssemblyExtensions.GetProgramProduct()} Report API", Version = V1ApiController.API_VERSION });

                var restXmlDoc = Path.Combine(AppContext.BaseDirectory, "FS.TimeTracking.Report.Api.REST.xml");
                var abstractionsXmlDoc = Path.Combine(AppContext.BaseDirectory, "FS.TimeTracking.Report.Abstractions.xml");

                c.OperationFilter<AddCSharpActionFilter>();
                c.IncludeXmlComments(restXmlDoc);
                c.IncludeXmlComments(abstractionsXmlDoc);
            });

    internal static WebApplication RegisterOpenApiRoutes(this WebApplication webApplication)
    {
        webApplication
            .UseSwagger(c => c.RouteTemplate = $"{OPEN_API_UI_ROUTE}{{documentName}}/{OPEN_API_SPEC}")
            .UseSwaggerUI(c =>
            {
                c.RoutePrefix = OPEN_API_UI_ROUTE.Trim('/');
                c.SwaggerEndpoint($"{V1ApiController.API_VERSION}/{OPEN_API_SPEC}", $"API version {V1ApiController.API_VERSION}");
                c.DisplayRequestDuration();
            });

        return webApplication;
    }

    internal static void GenerateOpenApiSpec(this IHost host, string outFile)
    {
        if (string.IsNullOrWhiteSpace(outFile))
            throw new ArgumentException("No destination file for generated OpenAPI document given.");

        var openApiJson = host
            .Services
            .GetRequiredService<ISwaggerProvider>()
            .GetSwagger(V1ApiController.API_VERSION)
            .SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

        File.WriteAllText(outFile, openApiJson);
    }
}