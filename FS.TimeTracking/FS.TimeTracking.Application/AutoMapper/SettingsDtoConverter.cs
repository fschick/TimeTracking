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
        => source
            .ToDictionary(x => x.Key, x => x.Value)
            .ToObject<SettingDto>();
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class SettingsFromDtoMapper : ITypeConverter<SettingDto, List<Setting>>
{
    public List<Setting> Convert(SettingDto source, List<Setting> destination, ResolutionContext context)
        => source
            .ToDictionary()
            .Select(x => new Setting { Key = x.Key, Value = x.Value })
            .ToList();
}