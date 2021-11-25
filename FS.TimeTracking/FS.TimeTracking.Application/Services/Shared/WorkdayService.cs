using FS.FilterExpressionCreator.Models;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Shared;
using FS.TimeTracking.Shared.Enums;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Shared;
/// <inheritdoc />
public class WorkdayService : IWorkdayService
{
    private readonly ISettingService _settingService;
    private readonly AsyncLazy<List<HolidayDto>> _holidays;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkdayService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="settingService">The setting service.</param>
    public WorkdayService(IRepository repository, ISettingService settingService)
    {
        _settingService = settingService;
        _holidays = new AsyncLazy<List<HolidayDto>>(async () => await repository.Get<Holiday, HolidayDto>());
    }

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => await GetWorkdays(startDate.GetDays(endDate), cancellationToken);

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(DateTimeSpan? dateTimeSpan, CancellationToken cancellationToken = default)
        => dateTimeSpan != null ? await GetWorkdays(dateTimeSpan.Value.Start.Date, dateTimeSpan.Value.End.Date, cancellationToken) : null;

    ///// <inheritdoc />
    //public async Task<int> GetWorkDaysCount(DateTime startDate, DateTime endDate)
    //    => (await GetWorkDays(startDate, endDate)).Count();

    ///// <inheritdoc />
    //public async Task<IEnumerable<DateTime>> GetWorkDaysOfMonth(int year, int month)
    //    => GetWorkingDays(new DateTime(year, month, 1).GetDaysOfMonth());

    ///// <inheritdoc />
    //public async Task<int> GetWorkDaysCountOfMonth(int year, int month)
    //    => (await GetWorkDaysOfMonth(year, month)).Count();

    ///// <inheritdoc />
    //public async Task<IEnumerable<DateTime>> GetWorkDaysOfMonthTillDay(int year, int month, int day)
    //    => GetWorkingDays(new DateTime(year, month, day).GetDaysOfMonthTillDay());

    ///// <inheritdoc />
    //public async Task<int> GetWorkDaysCountOfMonthTillDay(int year, int month, int day)
    //    => (await GetWorkDaysOfMonthTillDay(year, month, day)).Count();

    private async Task<WorkdaysDto> GetWorkdays(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.Get(cancellationToken);
        var holidays = await _holidays;

        var workdays = settings.Workdays
            .Where(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        var publicHolidayDates = holidays
            .Where(x => x.Type == HolidayType.PublicHoliday)
            .SelectMany(x => x.StartDate.Date.GetDays(x.EndDate.Date))
            .Distinct();

        var personalHolidayDates = holidays
            .Where(x => x.Type == HolidayType.Holiday)
            .SelectMany(x => x.StartDate.Date.GetDays(x.EndDate.Date))
            .Distinct();

        var publicWorkdays = dates
            .Where(date => workdays.Contains(date.DayOfWeek))
            .Except(publicHolidayDates)
            .ToList();

        var personalWorkdays = publicWorkdays
            .Except(personalHolidayDates)
            .ToList();

        return new WorkdaysDto
        {
            PublicWorkdays = publicWorkdays,
            PersonalWorkdays = personalWorkdays
        };
    }
}