using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace FS.TimeTracking.Api.REST.Filters;

/// <summary>
/// Removes disabled features from OpenAPI specification.
/// </summary>
public class FeatureGateDocumentFilter : IDocumentFilter
{
    private readonly IFeatureManager _featureManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureGateDocumentFilter"/> class.
    /// </summary>
    /// <param name="featureManager">Manager for feature.</param>
    public FeatureGateDocumentFilter(IFeatureManager featureManager)
        => _featureManager = featureManager;

    /// <summary>
    /// Apply filter rules.
    /// </summary>
    /// <param name="swaggerDoc">The swagger document.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var apiDescription in context.ApiDescriptions)
        {
            if (apiDescription.RelativePath == null)
                continue;

            var filterPipeline = apiDescription.ActionDescriptor.FilterDescriptors;
            var filterMetaData = filterPipeline
                .Select(filterInfo => filterInfo.Filter)
                .SingleOrDefault(filter => filter is FeatureGateAttribute);
            if (filterMetaData == default)
                continue;

            var featureGateAttribute = filterMetaData as FeatureGateAttribute;
            var isActive = featureGateAttribute != null && _featureManager
                .IsEnabledAsync(featureGateAttribute.Features.Single())
                .GetAwaiter().GetResult();
            if (isActive)
                continue;

            var apiPath = swaggerDoc.Paths.FirstOrDefault(o => o.Key.Contains(apiDescription.RelativePath));
            swaggerDoc.Paths.Remove(apiPath.Key);
        }
    }
}