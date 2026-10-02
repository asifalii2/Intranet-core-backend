using AutoMapper;
using Intranet.Core.DTOs.Attachments;
using Intranet.Core.DTOs.Users;
using Intranet.Core.Helpers;
using Intranet.Core.Profiles.CustomResolvers;
using Intranet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Profiles
{
    public class AttachmentProfile: Profile
    {
        public AttachmentProfile() 
        {
            CreateMap<Attachment, AttachmentDto>();
            CreateMap<Attachment, AttachmentForListing>();
            CreateMap<AttachmentForCreation, Attachment>()
               .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
               .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => Path.GetExtension(src.File.FileName)))
               .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.File.ContentType))
               .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.File.Length))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
               .ForMember(dest => dest.CreatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<AttachmentForCreation, Attachment>>());
            CreateMap<AttachmentForUpdate, Attachment>()
               .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
               .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom<SetCurrentUserIdResolver<AttachmentForUpdate, Attachment>>());
        }
    }
}
