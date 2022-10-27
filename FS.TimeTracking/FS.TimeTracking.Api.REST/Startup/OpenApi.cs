using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using AssemblyExtensions = FS.TimeTracking.Core.Extensions.AssemblyExtensions;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class OpenApi
{
    public const string OPEN_API_UI_ROUTE = "openapi/";
    public const string OPEN_API_SPEC = "openapi.json";
    public const string SWAGGER_UI_ROUTE = "swagger/";
    public const string SECURITY_DEFINITION_NAME = "OAuth2";

    internal static IServiceCollection RegisterOpenApiController(this IServiceCollection services)
        => services
            .AddSwaggerGenNewtonsoftSupport()
            .AddSwaggerGen(options =>
            {
                const string documentName = V1ApiController.API_VERSION;
                options.SwaggerDoc(documentName, new OpenApiInfo { Title = $"{AssemblyExtensions.GetProgramProduct()} API", Version = V1ApiController.API_VERSION });

                options.OperationFilter<AddCSharpActionFilter>();
                options.DocumentFilter<FeatureGateDocumentFilter>();

                var restXmlDoc = Path.Combine(AppContext.BaseDirectory, "FS.TimeTracking.Api.REST.xml");
                var abstractionsXmlDoc = Path.Combine(AppContext.BaseDirectory, "FS.TimeTracking.Abstractions.xml");
                options.IncludeXmlComments(restXmlDoc);
                options.IncludeXmlComments(abstractionsXmlDoc);

                options.AddFilterExpressionCreators(restXmlDoc, abstractionsXmlDoc);
            });

    internal static WebApplication RegisterOpenApiRoutes(this WebApplication webApplication)
    {
        webApplication
            .UseSwagger(c => c.RouteTemplate = $"{OPEN_API_UI_ROUTE}{{documentName}}/{OPEN_API_SPEC}")
            .UseSwaggerUI(options =>
            {
                options.RoutePrefix = OPEN_API_UI_ROUTE.Trim('/');
                options.SwaggerEndpoint($"{V1ApiController.API_VERSION}/{OPEN_API_SPEC}", $"API version {V1ApiController.API_VERSION}");
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnableFilter();
                options.EnableTryItOutByDefault();
                options.ConfigObject.AdditionalItems.Add("requestSnippetsEnabled", true);
            });

        return webApplication;
    }

    internal static void GenerateOpenApiSpec(this IHost host, string outFile)
    {
        if (string.IsNullOrWhiteSpace(outFile))
            throw new ArgumentException("No destination file for generated OpenAPI document given.");

        var openApiProvider = host.Services.GetRequiredService<ISwaggerProvider>();
        var openApiDocument = openApiProvider.GetSwagger(V1ApiController.API_VERSION);
        var openApiJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

        File.WriteAllText(outFile, openApiJson);
    }
}