using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Extensions;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Application.AutoMapper;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class SettingsToDtoConverter : ITypeConverter<List<Setting>, SettingDto>
{
    public SettingDto Convert(List<Setting> source, SettingDto destination, ResolutionContext context)
        => source
            .ToDictionary(x => x.Key, x => x.Value)
            .ToObject<SettingDto>();
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class SettingsFromDtoConverter : ITypeConverter<SettingDto, List<Setting>>
{
    public List<Setting> Convert(SettingDto source, List<Setting> destination, ResolutionContext context)
        => source
            .ToDictionary()
            .Select(x => new Setting { Key = x.Key, Value = x.Value })
            .ToList();
}