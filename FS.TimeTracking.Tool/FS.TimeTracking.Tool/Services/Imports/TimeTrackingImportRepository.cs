using AutoMapper;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Tool.DbContexts.Imports;
using FS.TimeTracking.Tool.Interfaces.Import;

namespace FS.TimeTracking.Tool.Services.Imports
{
    internal class TimeTrackingImportRepository : DbRepository<TimeTrackingImportDbContext>, ITimeTrackingImportRepository
    {
        public TimeTrackingImportRepository(TimeTrackingImportDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
