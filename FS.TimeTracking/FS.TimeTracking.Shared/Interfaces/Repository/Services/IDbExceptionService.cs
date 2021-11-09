using FS.TimeTracking.Shared.Models.Repository;
using System.Data.Common;

namespace FS.TimeTracking.Shared.Interfaces.Repository.Services
{
    /// <summary>
    /// Database specific exception handling
    /// </summary>
    public interface IDbExceptionService
    {
        /// <summary>
        /// Translates the database specific exception to an unified error code.
        /// </summary>
        /// <param name="dbException">The database exception.</param>
        DatabaseErrorCode TranslateDbException(DbException dbException);
    }
}