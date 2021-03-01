using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="ITimeSheetService" />
    public class TimeSheetService : CrudModelService<TimeSheet, TimeSheetDto, TimeSheetDto>, ITimeSheetService
    {
        /// <inheritdoc />
        public TimeSheetService(IRepository repository, IModelConverter<TimeSheet, TimeSheetDto> modelConverter)
            : base(repository, modelConverter)
        {
        }

        /// <inheritdoc />
        public override Task<List<TimeSheetDto>> List(Guid? id, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}
