using AutoMapper;
using Intranet.Core.Interfaces.Services;

namespace Intranet.Core.Profiles.CustomResolvers;

public class SetCurrentUserIdIfNullResolver<Source, Dest> :
        IValueResolver<Source, Dest, int?>
{
    private readonly IAuthUser _authUser;
    public SetCurrentUserIdIfNullResolver(IAuthUser authUser)
    {
        _authUser = authUser;
    }

    public int? Resolve(Source source, Dest destination, int? destMember, ResolutionContext context)
    {
        return _authUser.UserId < 1 ? null : _authUser.UserId;
    }
}


