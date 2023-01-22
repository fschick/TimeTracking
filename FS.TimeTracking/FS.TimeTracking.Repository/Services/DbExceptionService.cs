using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using System.Data.Common;

namespace FS.TimeTracking.Repository.Services;

/// <inheritdoc />
public class DbExceptionService : IDbExceptionService
{
    /// <inheritdoc />
    public ApplicationErrorCode TranslateDbException(DbException dbException)
    {
        switch (dbException)
        {
            case SqliteException sqliteException when IsForeignKeyViolation(sqliteException):
            case SqlException sqlException when IsForeignKeyViolation(sqlException):
            case PostgresException postgresException when IsForeignKeyViolation(postgresException):
            case MySqlException mySqlException when IsForeignKeyViolation(mySqlException):
                return ApplicationErrorCode.ConflictForeignKeyViolation;
            default:
                return ApplicationErrorCode.Unknown;
        }
    }

    private static bool IsForeignKeyViolation(SqliteException exception)
        => exception.SqliteErrorCode == 19 /*SQLITE_CONSTRAINT*/ && exception.SqliteExtendedErrorCode == 1811 /*SQLITE_CONSTRAINT_TRIGGER*/;

    private static bool IsForeignKeyViolation(SqlException exception)
        // The %ls statement conflicted with the %ls constraint "%.*ls". The conflict occurred in database "%.*ls", table "%.*ls"%ls%.*ls%ls.
        => exception.Number == 547 && exception.Class == 16;

    private static bool IsForeignKeyViolation(PostgresException exception)
        => exception.SqlState == "23503" /* foreign_key_violation */;

    private static bool IsForeignKeyViolation(MySqlException exception)
        => exception.Number == 1451 && exception.SqlState == "23000"; /* ER_ROW_IS_REFERENCED_2 */
}