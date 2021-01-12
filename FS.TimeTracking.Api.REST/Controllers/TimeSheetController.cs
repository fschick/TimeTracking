using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <inheritdoc cref="ITimeSheetService" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="ITimeSheetService" />
    [V1ApiController]
    public class TimeSheetController : CrudModelController<TimeSheetDto>, ITimeSheetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSheetController"/> class.
        /// </summary>
        /// <param name="modelService">The model service.</param>
        public TimeSheetController(ITimeSheetService modelService)
            : base(modelService)
        {
        }
    }
}
