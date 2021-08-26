using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Repository.DbFunctions
{
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
        }

        /// <summary>
        /// Registers interceptors for custom date time functions in SQLite.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        public static void RegisterSqLiteDateTimeFunctions(this DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(new SqLiteDateTimeFunctionsInterceptor());

        private static void RegisterMethodToUtc(this ModelBuilder modelBuilder, DatabaseType databaseType)
        {
            var toUtc = typeof(DateTimeExtensions)
                .GetMethods()
                .Single(x => x.Name == nameof(DateTimeExtensions.ToUtc) && x.ReturnType == typeof(DateTime));

            modelBuilder
                .HasDbFunction(toUtc)
                .HasTranslation(arguments
                    => new SqlFunctionExpression(
                        schema: GetFunctionSchema(databaseType),
                        functionName: GetFunctionName(nameof(DateTimeExtensions.ToUtc), databaseType),
                        arguments: arguments,
                        nullable: false,
                        argumentsPropagateNullability: new[] { false, false },
                        type: typeof(DateTime),
                        typeMapping: null
                    )
                );
        }

        private static void RegisterMethodToUtcNullable(this ModelBuilder modelBuilder, DatabaseType databaseType)
        {
            var toUtc = typeof(DateTimeExtensions)
                .GetMethods()
                .Single(x => x.Name == nameof(DateTimeExtensions.ToUtc) && x.ReturnType == typeof(DateTime?));

            modelBuilder
                .HasDbFunction(toUtc)
                .HasTranslation(arguments
                    => new SqlFunctionExpression(
                        schema: GetFunctionSchema(databaseType),
                        functionName: GetFunctionName(nameof(DateTimeExtensions.ToUtc), databaseType),
                        arguments: arguments,
                        nullable: true,
                        argumentsPropagateNullability: new[] { true, false },
                        type: typeof(DateTime?),
                        typeMapping: null
                    )
                );
        }
        private static string GetFunctionSchema(DatabaseType databaseType)
            => databaseType switch
            {
                DatabaseType.SqLite => null,
                DatabaseType.SqlServer => "dbo",
                DatabaseType.PostgreSql => null,
                DatabaseType.MySql => null,
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null)
            };

        private static string GetFunctionName(string functionName, DatabaseType databaseType)
            => databaseType switch
            {
                DatabaseType.SqLite => functionName,
                DatabaseType.SqlServer => functionName,
                DatabaseType.MySql => functionName,
                DatabaseType.PostgreSql => functionName.ToLowerInvariant(),
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null)
            };

        private class SqLiteDateTimeFunctionsInterceptor : DbConnectionInterceptor
        {
            public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
            {
                base.ConnectionOpened(connection, eventData);
                CreateFunctionToUtc((SqliteConnection)connection);
                CreateFunctionToUtcNullable((SqliteConnection)connection);
            }

            public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
            {
                var result = base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
                CreateFunctionToUtc((SqliteConnection)connection);
                CreateFunctionToUtcNullable((SqliteConnection)connection);
                return result;
            }

            private static void CreateFunctionToUtc(SqliteConnection connection)
                => connection.CreateFunction(nameof(DateTimeExtensions.ToUtc), (Func<DateTime, int, DateTime>)DateTimeExtensions.ToUtc, isDeterministic: true);

            private static void CreateFunctionToUtcNullable(SqliteConnection connection)
                => connection.CreateFunction(nameof(DateTimeExtensions.ToUtc), (Func<DateTime?, int, DateTime?>)DateTimeExtensions.ToUtc, isDeterministic: true);
        }
    }
}
