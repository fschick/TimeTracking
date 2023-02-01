using FS.TimeTracking.Abstractions.Attributes;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Validation description services
/// </summary>
public interface IValidationDescriptionApiService
{
    /// <summary>
    /// Gets a JSON with the validation rules of models marked with <see cref="ValidationDescriptionAttribute"/>.
    /// </summary>
    Task<JObject> GetValidationDescriptions();
}