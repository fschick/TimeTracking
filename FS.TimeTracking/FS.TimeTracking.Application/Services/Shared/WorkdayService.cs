using FS.FilterExpressionCreator.Abstractions.Models;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Abstractions.Interfaces.Repository.Services;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Shared.Extensions;
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
    public async Task<WorkdaysDto> GetWorkdays(Range<DateTimeOffset> dateTimeRange, CancellationToken cancellationToken = default)
        => dateTimeRange != null
            ? await GetWorkdays(dateTimeRange.Start.Date, dateTimeRange.End.Date, cancellationToken)
            : null;

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => startDate <= endDate
            ? await GetWorkdays(startDate.GetDays(endDate), cancellationToken)
            : new WorkdaysDto { PublicWorkdays = new(), PersonalWorkdays = new() };

    private async Task<WorkdaysDto> GetWorkdays(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
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