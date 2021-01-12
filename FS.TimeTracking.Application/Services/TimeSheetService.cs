using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.Services
{
    public class TimeSheetService : CrudModelService<TimeSheet, TimeSheetDto>, ITimeSheetService
    {
        public TimeSheetService(IRepository repository, IModelConverter<TimeSheet, TimeSheetDto> modelConverter)
            : base(repository, modelConverter)
        {
        }
    }
}
