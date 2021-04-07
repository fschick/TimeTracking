﻿using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Shared.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace FS.TimeTracking.Repository.Services
{
    /// <inheritdoc />
    public class TruncateDbService : ITruncateDbService
    {
        private readonly TimeTrackingDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TruncateDbService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <autogeneratedoc />
        public TruncateDbService(TimeTrackingDbContext dbContext)
            => _dbContext = dbContext;

        /// <inheritdoc />
        public void TruncateDatabase()
        {
            var sqlGenerator = _dbContext.GetInfrastructure().GetRequiredService<IMigrationsSqlGenerator>();
            var connection = _dbContext.GetInfrastructure().GetRequiredService<IRelationalConnection>();
            var closeConnection = !connection.Open();

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

            if (closeConnection)
                connection.Close();
        }

        private List<Table> GetTables(DbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = GetTableSchemaSelectionString();
            using var reader = command.ExecuteReader();
            return reader
                .Cast<IDataRecord>()
                .Select(x => new Table
                {
                    Schema = x.IsDBNull(0) ? null : x.GetString(0),
                    Name = x.IsDBNull(1) ? null : x.GetString(1)
                })
                .ToList();
        }

        private string GetTableSchemaSelectionString()
        {
            if (_dbContext.Database.IsSqlite())
                return "SELECT 'main', name FROM main.sqlite_master WHERE type = 'table'";
            if (_dbContext.Database.IsSqlServer())
                return "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
            if (_dbContext.Database.IsNpgsql())
                return "SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'";
            if (_dbContext.Database.IsMySql())
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
                return $"SELECT NULL, TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA = '{_dbContext.Database.GetDbConnection().Database}' AND TABLE_TYPE  = 'BASE TABLE'";
#pragma warning restore IDISP004 // Don't ignore created IDisposable.

            throw new NotImplementedException("Table information selection command for current database type implemented");
        }

        private class Table
        {
            public string Schema { get; set; }
            public string Name { get; set; }
        }
    }
}