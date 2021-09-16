using FS.TimeTracking.Shared.Attributes;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services
{
    /// <summary>
    /// Validation description services
    /// </summary>
    public interface IValidationDescriptionService
    {
        /// <summary>
        /// Gets a JSON object containing the validation rules descriptions of models marked with <see cref="ValidationDescriptionAttribute"/>.
        /// </summary>
        Task<JObject> GetValidationDescriptions();
    }
}