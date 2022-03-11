using FS.TimeTracking.Shared.Attributes;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;

/// <summary>
/// Validation description services
/// </summary>
public interface IValidationDescriptionService
{
    /// <summary>
    /// Gets a JSON with the validation rules of models marked with <see cref="ValidationDescriptionAttribute"/>.
    /// </summary>
    Task<JObject> GetValidationDescriptions();
}