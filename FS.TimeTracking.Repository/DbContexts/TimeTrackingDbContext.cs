﻿using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Shared.Models.TimeTracking;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace FS.TimeTracking.Repository.DbContexts
{
    /// <inheritdoc />
    public class TimeTrackingDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly TimeTrackingConfiguration _configuration;
        private readonly EnvironmentConfiguration _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingDbContext"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        /// <autogeneratedoc />
        public TimeTrackingDbContext(ILoggerFactory loggerFactory, TimeTrackingConfiguration configuration, EnvironmentConfiguration environment)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _environment = environment;
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            LinqToDBForEFTools.Initialize();

            options
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging(_environment.IsDevelopment);

            var connectionString = _configuration.Database.ConnectionString;
            var repositoryAssemblyName = typeof(TimeTrackingDbContext).Assembly.GetName().Name;
            var migrationAssembly = $"{repositoryAssemblyName}.{_configuration.Database.Type}";

            switch (_configuration.Database.Type)
            {
                case DatabaseType.SqLite:
                    options.UseSqlite(connectionString, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                case DatabaseType.SqlServer:
                    options.UseSqlServer(connectionString, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                case DatabaseType.PostgreSql:
                    options.UseNpgsql(connectionString, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                case DatabaseType.MySql:
                    var serverVersion = ServerVersion.AutoDetect(connectionString);
                    options.UseMySql(connectionString, serverVersion, o => o.MigrationsAssembly(migrationAssembly));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCustomer(modelBuilder.Entity<Customer>());
            ConfigureProject(modelBuilder.Entity<Project>());
            ConfigureActivity(modelBuilder.Entity<Activity>());
            ConfigureOrder(modelBuilder.Entity<Order>());
            ConfigureTimeSheet(modelBuilder.Entity<TimeSheet>());

            RegisterDateTimeAsUtcConverter(modelBuilder);
        }

        private static void ConfigureCustomer(EntityTypeBuilder<Customer> customerBuilder)
        {
            customerBuilder
                .ToTable("Customers")
                .HasIndex(customer => new { customer.Title, customer.Hidden });
        }

        private static void ConfigureProject(EntityTypeBuilder<Project> projectBuilder)
        {
            projectBuilder
                .ToTable("Projects")
                .HasIndex(project => new { project.Title, project.Hidden });

            projectBuilder
                .HasOne(project => project.Customer)
                .WithMany(customer => customer.Projects)
                .HasForeignKey(project => project.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

        private static void ConfigureOrder(EntityTypeBuilder<Order> orderBuilder)
        {
            orderBuilder
                .ToTable("Orders")
                .HasIndex(project => new { project.Title, project.Hidden });

            orderBuilder
                .HasOne(project => project.Customer)
                .WithMany(customer => customer.Orders)
                .HasForeignKey(project => project.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

        private static void ConfigureActivity(EntityTypeBuilder<Activity> activityBuilder)
        {
            activityBuilder
                .ToTable("Activities")
                .HasIndex(activity => new { activity.Title, activity.Hidden });

            activityBuilder
                .HasOne(activity => activity.Project)
                .WithMany()
                .HasForeignKey(activity => activity.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureTimeSheet(EntityTypeBuilder<TimeSheet> timeSheetBuilder)
        {
            timeSheetBuilder
                .ToTable("TimeSheets");

            timeSheetBuilder
                .HasOne<Project>()
                .WithMany()
                .HasForeignKey(timeSheet => timeSheet.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            timeSheetBuilder
                .HasOne<Activity>()
                .WithMany()
                .HasForeignKey(timeSheet => timeSheet.ActivityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            timeSheetBuilder
                .HasOne<Order>()
                .WithMany()
                .HasForeignKey(timeSheet => timeSheet.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // https://stackoverflow.com/a/61243301/1271211
        private static void RegisterDateTimeAsUtcConverter(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>
            (
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>
            (
                v => v.HasValue ? v.Value.ToUniversalTime() : null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
            );

            var properties = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties())
                .ToList();

            foreach (var property in properties)
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
        }
    }
}
