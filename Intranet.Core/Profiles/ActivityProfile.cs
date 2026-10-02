using AutoMapper;
using Intranet.Core.DTOs.Activities;
using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.RecentActivityUser;
using Intranet.Core.Profiles.CustomResolvers;
using Intranet.Domain.Entities;
using System;

namespace Intranet.Core.Profiles
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<RecentActivityUser, ActivityForListing>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Activity.Id))
             .ForMember(dest => dest.ActionDescription, opt => opt.MapFrom(src => src.Activity.ActionDescription))
             .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.Activity.EntityType))
             .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.Activity.EntityId))
             .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.Activity.EntityName))
             .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Activity.CreatedBy))
             .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Activity.CreatedAt));
            CreateMap<Activity, ActivityForListing>()
                .ForMember(d => d.User, o => o.MapFrom(s => s.CreatedBy));
            CreateMap<ActivityForCreation, Activity>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<ActivityForCreation, Activity>>());
            CreateMap<RecentActivityUserForCreation, RecentActivityUser>();
        }
    }
}

