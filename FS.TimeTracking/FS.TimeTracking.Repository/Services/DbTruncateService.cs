﻿using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace FS.TimeTracking.Repository.Services;

/// <inheritdoc />
public class DbTruncateService : IDbTruncateService
{
    private readonly TimeTrackingDbContext _dbContext;
    private readonly TimeTrackingConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbTruncateService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="configuration">The configuration.</param>
    /// <autogeneratedoc />
    public DbTruncateService(TimeTrackingDbContext dbContext, IOptions<TimeTrackingConfiguration> configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public void TruncateDatabase()
    {
        var connection = _dbContext.GetInfrastructure().GetRequiredService<IRelationalConnection>();
        var closeConnection = !connection.Open();

        DropTables(connection);
        DropFunctions(connection);

        if (closeConnection)
            connection.Close();
    }

    private void DropTables(IRelationalConnection connection)
    {
        var sqlGenerator = _dbContext.GetInfrastructure().GetRequiredService<IMigrationsSqlGenerator>();
        var tables = GetTables(connection.DbConnection);
        var currentRetry = 0;
        var maxRetries = tables.Count;
        while (tables.Any() && currentRetry < maxRetries)
        {
            var tableDropOperations = tables
                .Select(table => new DropTableOperation { Schema = table.Schema, Name = table.Name, })
                .ToList();

            var migrationCommands = sqlGenerator.Generate(tableDropOperations);
            foreach (var migrationCommand in migrationCommands)
                try { migrationCommand.ExecuteNonQuery(connection); }
                catch { /* Ignore */}

            currentRetry++;
            tables = GetTables(connection.DbConnection);
        }
    }

    private List<DatabaseObject> GetTables(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = GetTableSchemaSelectionString();
        using var reader = command.ExecuteReader();
        return reader
            .Cast<IDataRecord>()
            .Select(x => new DatabaseObject
            {
                Schema = x.IsDBNull(0) ? null : x.GetString(0),
                Name = x.IsDBNull(1) ? null : x.GetString(1)
            })
            .ToList();
    }

    private string GetTableSchemaSelectionString()
        => _configuration.Database.Type switch
        {
            DatabaseType.InMemory or
            DatabaseType.Sqlite => "SELECT 'main', name FROM main.sqlite_master WHERE type = 'table'",
            DatabaseType.SqlServer => "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES",
            DatabaseType.PostgreSql => "SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'",
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
            DatabaseType.MySql => $"SELECT NULL, TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA = '{_dbContext.Database.GetDbConnection().Database}' AND TABLE_TYPE  = 'BASE TABLE'",
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
            _ => throw new ArgumentOutOfRangeException()
        };

    private void DropFunctions(IRelationalConnection connection)
    {
        var functions = GetFunctions(connection.DbConnection);
        if (functions == null)
            return;

        foreach (var function in functions)
        {
            using var command = connection.DbConnection.CreateCommand();
            command.CommandText = "DROP FUNCTION ";
            if (function.Schema != null)
                command.CommandText += function.Schema + '.';
            command.CommandText += function.Name;
            command.ExecuteNonQuery();
        }
    }

    private List<DatabaseObject> GetFunctions(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = GetFunctionsSelectionString();
        if (string.IsNullOrEmpty(command.CommandText))
            return null;

        using var reader = command.ExecuteReader();
        return reader
            .Cast<IDataRecord>()
            .Select(x => new DatabaseObject
            {
                Schema = x.IsDBNull(0) ? null : x.GetString(0),
                Name = x.IsDBNull(1) ? null : x.GetString(1)
            })
            .ToList();
    }

    private string GetFunctionsSelectionString()
        => _configuration.Database.Type switch
        {
            DatabaseType.InMemory or
            DatabaseType.Sqlite => null,
            DatabaseType.SqlServer => @"
                    SELECT '[dbo]' AS [Schema], '[' + name + ']' AS [Name]
                    FROM sys.sql_modules m 
                    INNER JOIN sys.objects o ON m.object_id=o.object_id
                    WHERE type_desc like '%function%'",
            DatabaseType.PostgreSql => @"
                    SELECT NULL AS Schema, format('%I.%I(%s)', ns.nspname, p.proname, oidvectortypes(p.proargtypes)) AS Name
                    FROM pg_proc p 
                    INNER JOIN pg_namespace ns ON (p.pronamespace = ns.oid)
                    WHERE ns.nspname = 'public';",
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
            DatabaseType.MySql => $@"
                    SELECT NULL, routine_name AS Name
                    FROM information_schema.routines
                    WHERE routine_schema NOT IN ('sys', 'mysql', 'information_schema', 'performance_schema')
                    AND routine_type = 'FUNCTION'
                    AND routine_schema = '{_dbContext.Database.GetDbConnection().Database}'",
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
            _ => throw new ArgumentOutOfRangeException()
        };

    private class DatabaseObject
    {
        public string Schema { get; set; }
        public string Name { get; set; }
    }
}