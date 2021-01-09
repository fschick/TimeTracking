using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace FS.TimeTracking.Repository.DbContexts
{
    public class TimeTrackingDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly TimeTrackingConfiguration _configuration;
        private readonly EnvironmentConfiguration _environment;

        public TimeTrackingDbContext(ILoggerFactory loggerFactory, TimeTrackingConfiguration configuration, EnvironmentConfiguration environment)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _environment = environment;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging(_environment.IsDevelopment);

            var connectionString = _configuration.Database.ConnectionString;
            var repositoryAssemblyName = typeof(TimeTrackingDbContext).Assembly.GetName().Name;
            var migrationAssembly = $"{repositoryAssemblyName}.{_configuration.Database.Type}";

            switch (_configuration.Database.Type)
            {
                case TimeTrackingConfiguration.DatabaseType.Memory:
                    options.UseInMemoryDatabase(connectionString);
                    break;
                case TimeTrackingConfiguration.DatabaseType.SqLite:
                    options.UseSqlite(connectionString, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                case TimeTrackingConfiguration.DatabaseType.SqlServer:
                    options.UseSqlServer(connectionString, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                case TimeTrackingConfiguration.DatabaseType.PostgreSql:
                    options.UseNpgsql(connectionString, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                case TimeTrackingConfiguration.DatabaseType.MySql:
                    var serverVersion = ServerVersion.AutoDetect(connectionString);
                    options.UseMySql(connectionString, serverVersion, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
