using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <seealso cref="ControllerBase" />
/// <seealso cref="IValidationDescriptionApiService" />
[ApiV1Controller]
[ExcludeFromCodeCoverage]
public class ValidationDescriptionController : ControllerBase, IValidationDescriptionApiService
{
    private readonly IValidationDescriptionApiService _validationDescriptionService;
    /// <summary>
    /// Initializes a new instance of the <see cref="IValidationDescriptionApiService"/> class.
    /// </summary>
    /// <param name="validationDescriptionService">The validation description service.</param>
    public ValidationDescriptionController(IValidationDescriptionApiService validationDescriptionService)
        => _validationDescriptionService = validationDescriptionService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<JObject> GetValidationDescriptions()
        => await _validationDescriptionService.GetValidationDescriptions();
}