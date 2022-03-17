using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Application.AutoMapper;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class SettingsDtoConverter : ITypeConverter<List<Setting>, SettingDto>
{
    public SettingDto Convert(List<Setting> source, SettingDto destination, ResolutionContext context)
    {
        var defaults = new SettingDto();

        var workdaysSrc = source.FirstOrDefault(x => x.Key == nameof(SettingDto.Workdays))?.Value;
        var workdays = workdaysSrc != null
            ? JsonConvert.DeserializeObject<Dictionary<DayOfWeek, bool>>(workdaysSrc)
            : defaults.Workdays;

        var workHoursPerWorkdaySrc = source.FirstOrDefault(x => x.Key == nameof(SettingDto.WorkHoursPerWorkday))?.Value;
        var workHoursPerWorkday = workHoursPerWorkdaySrc != null
            ? JsonConvert.DeserializeObject<TimeSpan>(workHoursPerWorkdaySrc)
            : defaults.WorkHoursPerWorkday;

        var currencySrc = source.FirstOrDefault(x => x.Key == nameof(SettingDto.Currency))?.Value;
        var currency = currencySrc != null
            ? JsonConvert.DeserializeObject<string>(currencySrc)
            : defaults.Currency;

        return new SettingDto
        {
            Workdays = workdays,
            WorkHoursPerWorkday = workHoursPerWorkday,
            Currency = currency
        };
    }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class SettingsFromDtoMapper : ITypeConverter<SettingDto, List<Setting>>
{
    public List<Setting> Convert(SettingDto source, List<Setting> destination, ResolutionContext context)
        => new()
        {
            new Setting { Key = nameof(SettingDto.Workdays), Value = JsonConvert.SerializeObject(source.Workdays) },
            new Setting { Key = nameof(SettingDto.WorkHoursPerWorkday), Value = JsonConvert.SerializeObject(source.WorkHoursPerWorkday) },
            new Setting { Key = nameof(SettingDto.Currency), Value = JsonConvert.SerializeObject(source.Currency) },
        };
}