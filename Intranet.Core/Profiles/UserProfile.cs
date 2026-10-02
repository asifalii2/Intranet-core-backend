using AutoMapper;
using Intranet.Core.DTOs.Users;
using Intranet.Core.Helpers;
using Intranet.Core.Profiles.CustomResolvers;
using Intranet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile() 
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserForListing>();
            CreateMap<User, UserOverviewDto>();
            CreateMap<User, UserSummaryDto>();

            CreateMap<UserForCreation, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.HasLoginEmailSent, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName!=null ? src.LastName.Trim() : src.LastName))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<UserForCreation, User>>());

            CreateMap<UserForUpdate, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<UserForUpdate, User>>());
            CreateMap<UserStatusForUpdate, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<UserStatusForUpdate, User>>());
            CreateMap<UserEmailTwoFactorEnabledStatusForUpdate, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<UserEmailTwoFactorEnabledStatusForUpdate, User>>());
            CreateMap<UserPhoneTwoFactorEnabledStatusForUpdate, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<UserPhoneTwoFactorEnabledStatusForUpdate, User>>());
        }
    }
}
