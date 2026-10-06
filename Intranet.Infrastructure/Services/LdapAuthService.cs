using Intranet.Core.Interfaces.Services;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Intranet.Infrastructure.Services
{
    /// <summary>
    /// Active Directory credential check — same intent as WebForms LDAP bind
    /// (DirectoryEntry against DC=halan,DC=pk). Uses Win32 LogonUser so no extra NuGet package is required.
    /// </summary>
    public class LdapAuthService : ILdapAuthService
    {
        private const int Logon32LogonNetwork = 3;
        private const int Logon32ProviderDefault = 0;

        private readonly ISettingsService _settingsService;

        public LdapAuthService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(
            string lpszUsername,
            string? lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        public bool Authenticate(string username, string password)
        {
            var ldapSettings = _settingsService.GetLdapSettings();

            if (!ldapSettings.Enabled)
                return true;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var domain = ResolveDomain(ldapSettings.Domain, ldapSettings.Path);
            IntPtr token = IntPtr.Zero;

            try
            {
                var success = LogonUser(
                    username,
                    domain,
                    password,
                    Logon32LogonNetwork,
                    Logon32ProviderDefault,
                    out token);

                if (!success)
                    return false;

                using var identity = new WindowsIdentity(token);
                return identity.IsAuthenticated;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (token != IntPtr.Zero)
                    CloseHandle(token);
            }
        }

        /// <summary>
        /// Prefer explicit Domain; otherwise derive from LDAP path like LDAP://DC=halan,DC=pk → halan.
        /// </summary>
        private static string ResolveDomain(string? configuredDomain, string? ldapPath)
        {
            if (!string.IsNullOrWhiteSpace(configuredDomain))
                return configuredDomain.Trim();

            if (string.IsNullOrWhiteSpace(ldapPath))
                return "halan";

            // LDAP://DC=halan,DC=pk  or  DC=halan,DC=pk
            var path = ldapPath.Replace("LDAP://", string.Empty, StringComparison.OrdinalIgnoreCase);
            var parts = path.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var dc = parts.FirstOrDefault(p => p.StartsWith("DC=", StringComparison.OrdinalIgnoreCase));
            if (dc != null)
                return dc[3..];

            return "halan";
        }
    }
}
