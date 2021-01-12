﻿using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Shared.Models.TimeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

namespace FS.TimeTracking.Repository.DbContexts
{
    public class TimeTrackingDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly TimeTrackingConfiguration _configuration;
        private readonly EnvironmentConfiguration _environment;

        public DbSet<Customer> Customers { get; set; }

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

            ConfigureCustomer(modelBuilder.Entity<Customer>());
            ConfigureProject(modelBuilder.Entity<Project>());
            ConfigureActivity(modelBuilder.Entity<Activity>());
            ConfigureTimeSheet(modelBuilder.Entity<TimeSheet>());
        }

        private static void ConfigureCustomer(EntityTypeBuilder<Customer> customerBuilder)
        {
            customerBuilder
                .HasIndex(x => new { x.ShortName, x.Hidden });
        }

        private static void ConfigureProject(EntityTypeBuilder<Project> projectBuilder)
        {
            projectBuilder
                .HasIndex(x => new { x.Name, x.Hidden });

            projectBuilder
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

        private static void ConfigureActivity(EntityTypeBuilder<Activity> activityBuilder)
        {
            activityBuilder
                .HasIndex(x => new { x.Name, x.Hidden });

            activityBuilder
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            activityBuilder
                .HasOne<Project>()
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureTimeSheet(EntityTypeBuilder<TimeSheet> timeSheetBuilder)
        {
            timeSheetBuilder
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            timeSheetBuilder
                .HasOne<Activity>()
                .WithMany()
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
