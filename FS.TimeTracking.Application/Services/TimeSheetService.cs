using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc cref="ITimeSheetService" />
    public class TimeSheetService : CrudModelService<TimeSheet, TimeSheetDto, TimeSheetListDto>, ITimeSheetService
    {
        /// <inheritdoc />
        public TimeSheetService(IRepository repository, IMapper mapper)
            : base(repository, mapper)
        { }

        /// <inheritdoc />
        public override async Task<List<TimeSheetListDto>> List(Guid? id = null, CancellationToken cancellationToken = default)
            => (await base.List(id, cancellationToken)).OrderBy(x => x.StartDate).ToList();
    }
}
