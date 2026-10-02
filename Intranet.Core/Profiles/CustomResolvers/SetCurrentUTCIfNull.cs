using AutoMapper;

namespace Intranet.Core.Profiles.CustomResolvers;

public class SetCurrentUTCIfNull<Source, Dest> :
         IValueResolver<Source, Dest, DateTime>
{
    public DateTime Resolve(Source source, Dest destination, DateTime destMember, ResolutionContext context)
    {

        if (destMember != default(DateTime))
        {
            return destMember;
        }

        return DateTime.UtcNow;
    }
}


