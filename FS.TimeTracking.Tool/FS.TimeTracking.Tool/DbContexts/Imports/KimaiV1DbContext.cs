﻿using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Tool.Models.Configurations;
using FS.TimeTracking.Tool.Models.Imports;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace FS.TimeTracking.Tool.DbContexts.Imports
{
    /// <inheritdoc />
    public class KimaiV1DbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _connectionString;
        private readonly DatabaseType _databaseType;
        private readonly string _tablePrefix;

        private readonly EnvironmentConfiguration _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingDbContext"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        /// <autogeneratedoc />
        public KimaiV1DbContext(ILoggerFactory loggerFactory, IOptions<KimaiV1ImportConfiguration> configuration, EnvironmentConfiguration environment)
        {
            _loggerFactory = loggerFactory;
            _connectionString = configuration.Value.ConnectionString;
            _databaseType = configuration.Value.DatabaseType;
            _tablePrefix = configuration.Value.TablePrefix.TrimEnd('_');
            _environment = environment;
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            LinqToDBForEFTools.Initialize();

            optionsBuilder
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging(_environment.IsDevelopment);

            switch (_databaseType)
            {
                case DatabaseType.SqLite:
                    optionsBuilder.UseSqlite(_connectionString);
                    break;
                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;
                case DatabaseType.PostgreSql:
                    optionsBuilder.UseNpgsql(_connectionString);
                    break;
                case DatabaseType.MySql:
                    var serverVersion = ServerVersion.AutoDetect(_connectionString);
                    optionsBuilder.UseMySql(_connectionString, serverVersion);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null, "Configured database type is unsupported");
            }
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCustomer(modelBuilder.Entity<KimaiV1Customer>());
            ConfigureProject(modelBuilder.Entity<KimaiV1Project>());
            ConfigureActivity(modelBuilder.Entity<KimaiV1Activity>());
            ConfigureProjectActivity(modelBuilder.Entity<KimaiV1ProjectActivity>());
            ConfigureTimeSheet(modelBuilder.Entity<KimaiV1TimeSheet>());
        }

        private void ConfigureCustomer(EntityTypeBuilder<KimaiV1Customer> customerBuilder)
        {
            customerBuilder
                .ToTable($"{_tablePrefix}_customers")
                .HasKey(x => x.CustomerId);
        }

        private void ConfigureProject(EntityTypeBuilder<KimaiV1Project> projectBuilder)
        {
            projectBuilder
                .ToTable($"{_tablePrefix}_projects")
                .HasKey(x => x.ProjectId);

            projectBuilder
                .HasOne(project => project.Customer)
                .WithMany()
                .HasForeignKey(project => project.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

        private void ConfigureActivity(EntityTypeBuilder<KimaiV1Activity> activityBuilder)
        {
            activityBuilder
                .ToTable($"{_tablePrefix}_activities")
                .HasKey(x => x.ActivityId);
        }

        private void ConfigureProjectActivity(EntityTypeBuilder<KimaiV1ProjectActivity> projectActivityBuilder)
        {
            projectActivityBuilder
                .ToTable($"{_tablePrefix}_projects_activities")
                .HasKey(x => new { x.ProjectId, x.ActivityId });

            projectActivityBuilder
                .HasOne(projectActivity => projectActivity.Project)
                .WithMany()
                .HasForeignKey(project => project.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            projectActivityBuilder
                .HasOne(projectActivity => projectActivity.Activity)
                .WithMany(x => x.Projects)
                .HasForeignKey(project => project.ActivityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }

        private void ConfigureTimeSheet(EntityTypeBuilder<KimaiV1TimeSheet> timeSheetBuilder)
        {
            timeSheetBuilder
                .ToTable($"{_tablePrefix}_timeSheet")
                .HasKey(x => x.TimeEntryId);

            timeSheetBuilder
                .HasOne<KimaiV1Project>()
                .WithMany()
                .HasForeignKey(timeSheet => timeSheet.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            timeSheetBuilder
                .HasOne<KimaiV1Activity>()
                .WithMany()
                .HasForeignKey(timeSheet => timeSheet.ActivityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}