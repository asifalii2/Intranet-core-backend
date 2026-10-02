using AutoMapper;
using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.NotificationUser;
using Intranet.Core.Profiles.CustomResolvers;
using Intranet.Domain.Entities;

namespace Intranet.Core.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationUser, NotificationForListing>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Notification.Id))
                 .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Notification.Title))
                 .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Notification.Message))
                 .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.Notification.EntityId))
                 .ForMember(dest => dest.Entity, opt => opt.MapFrom(src => src.Notification.Entity))
                 .ForMember(dest => dest.Blob, opt => opt.MapFrom(src => src.Notification.Blob))
                 .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Notification.CreatedAt));
            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationForCreation, Notification>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom<SetCurrentUserIdIfNullResolver<NotificationForCreation, Notification>>());
            CreateMap<NotificationUserForCreation, NotificationUser>();
        }
    }
}
