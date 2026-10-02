using AutoMapper;
using Intranet.Core.Interfaces.Services;

namespace Intranet.Core.Profiles.CustomResolvers;

public class SetCurrentUserIdResolver<Source, Dest> :
         IValueResolver<Source, Dest, int?>,
     IValueResolver<Source, Dest, int>
{
    private readonly IAuthUser _authUser;
    public SetCurrentUserIdResolver(IAuthUser authUser)
    {
        _authUser = authUser;
    }

    public int? Resolve(Source source, Dest destination, int? destMember, ResolutionContext context)
    {
        return _authUser.UserId;
    }
    public int Resolve(Source source, Dest destination, int destMember, ResolutionContext context)
    {
        return _authUser.UserId;
    }
}


