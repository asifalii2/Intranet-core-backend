using Intranet.Core.Profiles;
using Intranet.Core.Profiles.CustomResolvers;

namespace Intranet.Helpers
{
    public static class RegisterAutoMapperProfilesHelper
    {
        public static IServiceCollection RegisterProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<AuthProfile>();
                cfg.AddProfile<AttachmentProfile>();
                cfg.AddProfile<ActivityProfile>();
                cfg.AddProfile<NotificationProfile>();
            });
            // Resolvers with DI dependencies
            services.AddScoped(typeof(SetCurrentUserIdResolver<,>));
            services.AddScoped(typeof(SetCurrentUserIdIfNullResolver<,>));

            // No DI dependencies but register for consistency
            services.AddScoped(typeof(SetCurrentUTCIfNull<,>));
            return services;
        }
    }
}
