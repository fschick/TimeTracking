using FS.TimeTracking.Shared.DTOs.Shared;
using System;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;

/// <summary>
/// Workday services
/// </summary>
public interface IWorkdayService
{
    /// <summary>
    /// Gets the workdays for a given date/time range.
    /// </summary>
    /// <param name="startDate">The start date to get the workdays for.</param>
    /// <param name="endDate">The end date to get the workdays for.</param>
    /// <returns>Enumerable with one entry per per working day.</returns>
    Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate);

    ///// <summary>
    ///// Gets the count of workdays of a given month.
    ///// </summary>
    ///// <param name="startDate">The start date to get the workdays for.</param>
    ///// <param name="endDate">The end date to get the workdays for.</param>
    ///// <returns>The count of workdays.</returns>
    //Task<int> GetWorkDaysCount(DateTime startDate, DateTime endDate);

    ///// <summary>
    ///// Gets the workdays for a given month.
    ///// </summary>
    ///// <param name="year">The year to get the workdays for.</param>
    ///// <param name="month">The month to get the workdays for.</param>
    ///// <returns>Enumerable with one entry per per working day.</returns>
    //Task<IEnumerable<DateTime>> GetWorkDaysOfMonth(int year, int month);

    ///// <summary>
    ///// Gets the count of workdays for a given month.
    ///// </summary>
    ///// <param name="year">The year to get the count of workdays for.</param>
    ///// <param name="month">The month to get the count of workdays for.</param>
    ///// <returns>The count of workdays.</returns>
    //Task<int> GetWorkDaysCountOfMonth(int year, int month);

    ///// <summary>
    ///// Gets the workdays for a given month till a specific day of the month.
    ///// </summary>
    ///// <param name="year">Year the workdays are retrieved for.</param>
    ///// <param name="month">Month the workdays are retrieved for.</param>
    ///// <param name="day">The day till workdays are retrieved.</param>
    ///// <returns>Enumerable with one entry per per working day.</returns>
    //Task<IEnumerable<DateTime>> GetWorkDaysOfMonthTillDay(int year, int month, int day);

    ///// <summary>
    ///// Gets the count of workdays for a given month till a specific day of the month.
    ///// </summary>
    ///// <param name="year">Year the workdays are retrieved for.</param>
    ///// <param name="month">Month the workdays are retrieved for.</param>
    ///// <param name="day">The day till workdays are retrieved.</param>
    ///// <returns>The count of workdays.</returns>
    //Task<int> GetWorkDaysCountOfMonthTillDay(int year, int month, int day);
}