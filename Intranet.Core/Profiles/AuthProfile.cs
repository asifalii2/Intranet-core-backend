using AutoMapper;
using Intranet.Core.DTOs.Auth;
using Intranet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<User, LoginResponse>();
        }
    }
}
