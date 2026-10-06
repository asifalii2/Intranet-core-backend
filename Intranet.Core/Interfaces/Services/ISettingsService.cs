using Intranet.Core.DTOs.Settings;
using QuranFocusGroup.Core.DTOs.Settings;

namespace Intranet.Core.Interfaces.Services;

public interface ISettingsService
{
    JWTAuthenticationSettings GetJWTAuthenticationSettings();
    FrontendPortalUrlSettings GetFrontendPortalUrlSettings();
    EmailSettings GetEmailSettings();
    SecuritySettings GetSecuritySettings();
    LdapSettings GetLdapSettings();
    TwilioSettings GetTwilioSettings();
    AwsS3Settings GetAwsS3Settings();
    FirebaseSettings GetFirebaseSettings();
}

