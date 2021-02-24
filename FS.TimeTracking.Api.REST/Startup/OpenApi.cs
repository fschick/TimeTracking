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
using AssemblyExtensions = FS.TimeTracking.Shared.Extensions.AssemblyExtensions;

namespace FS.TimeTracking.Api.REST.Startup
{
    internal static class OpenApi
    {
        public const string OPEN_API_UI_ROUTE = "openapi/";
        public const string OPEN_API_SPEC = "openapi.json";
        public const string SWAGGER_UI_ROUTE = "swagger/";

        internal static IApplicationBuilder RegisterOpenApiRoutes(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseSwagger(c => c.RouteTemplate = $"{OPEN_API_UI_ROUTE}{{documentName}}/{OPEN_API_SPEC}")
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = OPEN_API_UI_ROUTE.Trim('/');
                    c.SwaggerEndpoint($"{V1ApiController.API_VERSION}/{OPEN_API_SPEC}", $"API version {V1ApiController.API_VERSION}");
                    c.DisplayRequestDuration();
                });

        internal static IServiceCollection RegisterOpenApiController(this IServiceCollection services)
            => services
                .AddSwaggerGenNewtonsoftSupport()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(V1ApiController.API_VERSION, new OpenApiInfo { Title = $"{AssemblyExtensions.GetProgramProduct()} API", Version = V1ApiController.API_VERSION });
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "FS.TimeTracking.Api.REST.xml"));
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "FS.TimeTracking.Shared.xml"));
                    c.OperationFilter<AddCSharpActionFilter>();
                });

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
}
