using Intranet.Core.DTOs.Settings;
using Intranet.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using QuranFocusGroup.Core.DTOs.Settings;

namespace Intranet.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IConfiguration _configuration = null!;
        public SettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public EmailSettings GetEmailSettings()
        {
            var settings = new EmailSettings();
            _configuration.GetSection("EmailSettings").Bind(settings);
            return settings;
        }

        public FrontendPortalUrlSettings GetFrontendPortalUrlSettings()
        {
            var settings = new FrontendPortalUrlSettings();
            _configuration.GetSection("FrontendPortalUrls").Bind(settings);
            return settings;
        }

        public JWTAuthenticationSettings GetJWTAuthenticationSettings()
        {
            var settings = new JWTAuthenticationSettings();
            _configuration.GetSection("Jwt").Bind(settings);
            return settings;
        }

        public SecuritySettings GetSecuritySettings()
        {
            var settings = new SecuritySettings();
            _configuration.GetSection("Security").Bind(settings);
            return settings;
        }

        public TwilioSettings GetTwilioSettings()
        {
            var settings = new TwilioSettings();
            _configuration.GetSection("Twilio").Bind(settings);
            return settings;
        }

        public AwsS3Settings GetAwsS3Settings()
        {
            var settings = new AwsS3Settings();
            _configuration.GetSection("AwsS3").Bind(settings);
            return settings;
        }

        public FirebaseSettings GetFirebaseSettings()
        {
            var settings = new FirebaseSettings();
            _configuration.GetSection("Firebase").Bind(settings);
            return settings;
        }
    }
}
