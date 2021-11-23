using AutoMapper;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.Models.MasterData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Application.AutoMapper;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class SettingsDtoMapper : ITypeConverter<List<Setting>, SettingDto>
{
    public SettingDto Convert(List<Setting> source, SettingDto destination, ResolutionContext context)
    {
        var defaults = SettingDto.Defaults;

        var workdaysSrc = source.FirstOrDefault(x => x.Key == nameof(SettingDto.Workdays))?.Value;
        var workdays = workdaysSrc != null
            ? JsonConvert.DeserializeObject<Dictionary<DayOfWeek, bool>>(workdaysSrc)
            : defaults.Workdays;

        var workHoursPerWorkdaySrc = source.FirstOrDefault(x => x.Key == nameof(SettingDto.WorkHoursPerWorkday))?.Value;
        var workHoursPerWorkday = workHoursPerWorkdaySrc != null
            ? JsonConvert.DeserializeObject<TimeSpan>(workHoursPerWorkdaySrc)
            : defaults.WorkHoursPerWorkday;

        return new SettingDto
        {
            Workdays = workdays,
            WorkHoursPerWorkday = workHoursPerWorkday
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
            new Setting { Key = nameof(SettingDto.WorkHoursPerWorkday), Value = JsonConvert.SerializeObject(source.WorkHoursPerWorkday) }
        };
}