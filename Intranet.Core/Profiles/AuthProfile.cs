using AutoMapper;
using Intranet.Core.DTOs.Auth;

namespace Intranet.Core.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<EmployeeLoginContext, LoginResponse>();
        }
    }
}
