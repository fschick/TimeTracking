using FS.TimeTracking.Shared.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;
using AssemblyExtensions = FS.TimeTracking.Shared.Extensions.AssemblyExtensions;

namespace FS.TimeTracking.Api.REST.Startup
{
    internal static class OpenApi
    {
        internal static IApplicationBuilder RegisterOpenApiRoutes(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseSwagger(c => c.RouteTemplate = $"{Routes.OpenApi.ROOT}{{documentName}}/{Routes.OpenApi.OpenApiSpec}")
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = Routes.OpenApi.OpenApiUi.Trim('/');
                    c.SwaggerEndpoint($"{Routes.API_VERSION}/{Routes.OpenApi.OpenApiSpec}", $"API version {Routes.API_VERSION}");
                });

        internal static IServiceCollection RegisterOpenApiController(this IServiceCollection services)
            => services
                .AddSwaggerGenNewtonsoftSupport()
                .AddSwaggerGen(c =>
                {
                    // Use method name as operationId
                    c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);
                    c.SwaggerDoc(Routes.API_VERSION, new OpenApiInfo { Title = $"{AssemblyExtensions.GetProgramProduct()} API", Version = Routes.API_VERSION });
                    c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "FS.TimeTracking.Api.REST.xml"));
                    c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "FS.TimeTracking.Shared.xml"));
                });

        internal static void GenerateOpenApiJson(this IHost host, string outFile)
        {
            if (string.IsNullOrWhiteSpace(outFile))
                throw new ArgumentException("No destination file for generated OpenAPI document given.");

            var openApiJson = host
                .Services
                .GetService<ISwaggerProvider>()
                .GetSwagger(Routes.API_VERSION)
                .SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            File.WriteAllText(outFile, openApiJson);
        }
    }
}
