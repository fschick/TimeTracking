using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="ITimeSheetService" />
    public class TimeSheetService : CrudModelService<TimeSheet, TimeSheetDto, TimeSheetDto>, ITimeSheetService
    {
        /// <inheritdoc />
        public TimeSheetService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }
    }
}
