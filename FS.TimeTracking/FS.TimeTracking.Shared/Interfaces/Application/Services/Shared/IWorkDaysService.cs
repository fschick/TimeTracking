using System;
using System.Collections.Generic;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.Shared
{
    /// <summary>
    /// Working days services
    /// </summary>
    public interface IWorkDaysService
    {
        /// <summary>
        /// Gets the working days for a given date/time range.
        /// </summary>
        /// <param name="startDate">The start date to get the working days for.</param>
        /// <param name="endDate">The end date to get the working days for.</param>
        /// <returns>Enumerable with one entry per per working day.</returns>
        IEnumerable<DateTime> GetWorkDays(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the count of working days of a given month.
        /// </summary>
        /// <param name="startDate">The start date to get the working days for.</param>
        /// <param name="endDate">The end date to get the working days for.</param>
        /// <returns>The count of working days.</returns>
        int GetWorkDaysCount(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the working days for a given month.
        /// </summary>
        /// <param name="year">The year to get the working days for.</param>
        /// <param name="month">The month to get the working days for.</param>
        /// <returns>Enumerable with one entry per per working day.</returns>
        IEnumerable<DateTime> GetWorkDaysOfMonth(int year, int month);

        /// <summary>
        /// Gets the count of working days for a given month.
        /// </summary>
        /// <param name="year">The year to get the count of working days for.</param>
        /// <param name="month">The month to get the count of working days for.</param>
        /// <returns>The count of working days.</returns>
        int GetWorkDaysCountOfMonth(int year, int month);

        /// <summary>
        /// Gets the working days for a given month till a specific day of the month.
        /// </summary>
        /// <param name="year">Year the working days are retrieved for.</param>
        /// <param name="month">Month the working days are retrieved for.</param>
        /// <param name="day">The day till working days are retrieved.</param>
        /// <returns>Enumerable with one entry per per working day.</returns>
        IEnumerable<DateTime> GetWorkDaysOfMonthTillDay(int year, int month, int day);

        /// <summary>
        /// Gets the count of working days for a given month till a specific day of the month.
        /// </summary>
        /// <param name="year">Year the working days are retrieved for.</param>
        /// <param name="month">Month the working days are retrieved for.</param>
        /// <param name="day">The day till working days are retrieved.</param>
        /// <returns>The count of working days.</returns>
        int GetWorkDaysCountOfMonthTillDay(int year, int month, int day);
    }
}
