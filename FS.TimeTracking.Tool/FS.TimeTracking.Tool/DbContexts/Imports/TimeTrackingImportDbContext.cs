using FS.TimeTracking.Abstractions.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FS.TimeTracking.Tool.DbContexts.Imports
{
    internal class TimeTrackingImportDbContext : TimeTrackingDbContext
    {
        public TimeTrackingImportDbContext(ILoggerFactory loggerFactory, IOptions<TimeTrackingConfiguration> configuration, EnvironmentConfiguration environment) :
            base(loggerFactory, configuration, environment)
        { }
    }
}
