using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Repository.DbFunctions;

/// <summary>
/// Hold methods to register custom date time functions in EF. 
/// </summary>
public static class DateTimeFunctions
{
    /// <summary>
    /// Registers the custom date time functions in EF.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="databaseType">Type of the database.</param>
    public static void RegisterDateTimeFunctions(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        RegisterMethodToUtc(modelBuilder, databaseType);
        RegisterMethodToUtcNullable(modelBuilder, databaseType);
        RegisterMethodDiffSeconds(modelBuilder, databaseType);
    }

    /// <summary>
    /// Registers interceptors for custom date time functions in SQLite.
    /// </summary>
    /// <param name="optionsBuilder">The options builder.</param>
    public static void RegisterSqliteDateTimeFunctions(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new SqliteDateTimeFunctionsInterceptor());

    private static void RegisterMethodToUtc(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        var toUtc = typeof(DateTimeExtensions)
            .GetMethods()
            .Single(x => x.Name == nameof(DateTimeExtensions.ToUtc) && x.ReturnType == typeof(DateTime));

        modelBuilder
            .HasDbFunction(toUtc)
            .HasSchema(GetFunctionSchema(databaseType))
            .HasName(GetFunctionName(nameof(DateTimeExtensions.ToUtc), databaseType))
            .IsNullable(false);
    }

    private static void RegisterMethodToUtcNullable(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        var toUtc = typeof(DateTimeExtensions)
            .GetMethods()
            .Single(x => x.Name == nameof(DateTimeExtensions.ToUtc) && x.ReturnType == typeof(DateTime?));

        modelBuilder
            .HasDbFunction(toUtc)
            .HasSchema(GetFunctionSchema(databaseType))
            .HasName(GetFunctionName(nameof(DateTimeExtensions.ToUtc), databaseType))
            .IsNullable(true);
    }

    private static void RegisterMethodDiffSeconds(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        var diffSeconds = typeof(DateTimeExtensions)
            .GetMethods()
            .Single(x => x.Name == nameof(DateTimeExtensions.DiffSeconds));

        modelBuilder
            .HasDbFunction(diffSeconds)
            .HasSchema(GetFunctionSchema(databaseType))
            .HasName(GetFunctionName(nameof(DateTimeExtensions.DiffSeconds), databaseType))
            .IsNullable(false);
    }

    private static string GetFunctionSchema(DatabaseType databaseType)
        => databaseType switch
        {
            DatabaseType.InMemory or
            DatabaseType.Sqlite => null,
            DatabaseType.SqlServer => "dbo",
            DatabaseType.PostgreSql => null,
            DatabaseType.MySql => null,
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null)
        };

    private static string GetFunctionName(string functionName, DatabaseType databaseType)
        => databaseType switch
        {
            DatabaseType.InMemory or
            DatabaseType.Sqlite => functionName,
            DatabaseType.SqlServer => functionName,
            DatabaseType.MySql => functionName,
            DatabaseType.PostgreSql => functionName.ToLowerInvariant(),
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null)
        };

    private class SqliteDateTimeFunctionsInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            base.ConnectionOpened(connection, eventData);
            CreateFunctionToUtc((SqliteConnection)connection);
            CreateFunctionToUtcNullable((SqliteConnection)connection);
            CreateFunctionDiffSeconds((SqliteConnection)connection);
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
            CreateFunctionToUtc((SqliteConnection)connection);
            CreateFunctionToUtcNullable((SqliteConnection)connection);
            CreateFunctionDiffSeconds((SqliteConnection)connection);
        }

        private static void CreateFunctionToUtc(SqliteConnection connection)
            => connection.CreateFunction(nameof(DateTimeExtensions.ToUtc), (Func<DateTime, int, DateTime>)DateTimeExtensions.ToUtc, isDeterministic: true);

        private static void CreateFunctionToUtcNullable(SqliteConnection connection)
            => connection.CreateFunction(nameof(DateTimeExtensions.ToUtc), (Func<DateTime?, int, DateTime?>)DateTimeExtensions.ToUtc, isDeterministic: true);

        private static void CreateFunctionDiffSeconds(SqliteConnection connection)
            => connection.CreateFunction(nameof(DateTimeExtensions.DiffSeconds), (Func<DateTime, int, DateTime?, int?, ulong>)DateTimeExtensions.DiffSeconds, isDeterministic: true);
    }
}