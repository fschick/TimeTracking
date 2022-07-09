namespace FS.TimeTracking.Abstractions.Enums.Report
{
    /// <summary>
    /// Report grouping member
    /// </summary>
    public enum ActivityReportGroup
    {
        /// <summary>
        /// Don't group the report
        /// </summary>
        None,

        /// <summary>
        /// Group the report by issue
        /// </summary>
        Issue,

        /// <summary>
        /// Group the report by order number
        /// </summary>
        OrderNumber
    }
}
