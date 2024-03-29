﻿using AutoMapper;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Tool.Models.Imports;
using System;

namespace FS.TimeTracking.Tool.AutoMapper.Imports;

/// <summary>
/// Configuration profile for auto mapper.
/// </summary>
/// <seealso cref="Profile" />
public class KimaiV1AutoMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KimaiV1AutoMapper"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public KimaiV1AutoMapper()
    {
        ClearPrefixes();

        CreateMap<string, string>().ConstructUsing(str => !string.IsNullOrWhiteSpace(str) ? str : null);

        var utcNow = DateTime.UtcNow;

        CreateMap<KimaiV1Customer, Customer>()
            .ForMember(x => x.Title, config => config.MapFrom(x => x.Name))
            .ForMember(x => x.CompanyName, config => config.MapFrom(x => x.Company))
            .ForMember(x => x.ContactName, config => config.MapFrom(x => x.Contact))
            .ForMember(x => x.Hidden, config => config.MapFrom(x => x.Trash || !x.Visible))
            .ForMember(x => x.Created, config => config.MapFrom(x => utcNow))
            .ForMember(x => x.Modified, config => config.MapFrom(x => utcNow));

        CreateMap<KimaiV1Project, Project>()
            .ForMember(x => x.Title, config => config.MapFrom(x => x.Name))
            .ForMember(x => x.Hidden, config => config.MapFrom(x => x.Trash || !x.Visible))
            .ForMember(x => x.CustomerId, config => config.Ignore())
            .ForMember(x => x.Created, config => config.MapFrom(x => utcNow))
            .ForMember(x => x.Modified, config => config.MapFrom(x => utcNow));

        CreateMap<KimaiV1Activity, Activity>()
            .ForMember(x => x.Title, config => config.MapFrom(x => x.Name))
            .ForMember(x => x.Hidden, config => config.MapFrom(x => x.Trash || !x.Visible))
            // TODO: Reactivate Project
            //.ForMember(x => x.ProjectId, config => config.Ignore())
            .ForMember(x => x.Created, config => config.MapFrom(x => utcNow))
            .ForMember(x => x.Modified, config => config.MapFrom(x => utcNow));

        CreateMap<KimaiV1TimeSheet, TimeSheet>()
            .ForMember(x => x.StartDate, config => config.MapFrom(x => DateTimeOffset.FromUnixTimeSeconds(x.Start)))
            .ForMember(x => x.EndDate, config => config.MapFrom(x => x.End != 0 ? DateTimeOffset.FromUnixTimeSeconds(x.End) : (DateTimeOffset?)null))
            .ForMember(x => x.Issue, config => config.MapFrom(x => x.TrackingNumber))
            .ForMember(x => x.Billable, config => config.MapFrom(x => !x.Billable.HasValue || x.Billable > 0))
            // TODO: Reactivate Project
            //.ForMember(x => x.ProjectId, config => config.Ignore())
            .ForMember(x => x.ActivityId, config => config.Ignore())
            .ForMember(x => x.Created, config => config.MapFrom(x => utcNow))
            .ForMember(x => x.Modified, config => config.MapFrom(x => utcNow));
    }
}