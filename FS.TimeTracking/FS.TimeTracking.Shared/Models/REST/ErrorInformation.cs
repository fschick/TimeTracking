using FS.TimeTracking.Shared.Models.Repository;

namespace FS.TimeTracking.Shared.Models.REST
{
    /// <summary>
    /// Contains extended error information about failed API requests
    /// </summary>
    public class ErrorInformation
    {
        /// <summary>
        /// A unified database error code.
        /// </summary>
        public DatabaseErrorCode DatabaseErrorCode { get; set; }
    }
}
