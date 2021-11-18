using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.MasterData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Application.AutoMapper
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    internal class SettingsToDtoMapper : ITypeConverter<List<Setting>, SettingDto>
    {
        public SettingDto Convert(List<Setting> source, SettingDto destination, ResolutionContext context)
            => new()
            {
                Workdays = JsonConvert.DeserializeObject<Dictionary<DayOfWeek, bool>>(source.First(x => x.Key == nameof(SettingDto.Workdays)).Value),
                WorkHoursPerWorkday = JsonConvert.DeserializeObject<TimeSpan>(source.First(x => x.Key == nameof(SettingDto.WorkHoursPerWorkday)).Value),
            };
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
}
