namespace Intranet.Core.DTOs.Settings
{
    public class LdapSettings
    {
        /// <summary>
        /// When false, LDAP/AD bind is skipped (local/dev only). Employee lookup still runs.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// LDAP path used by WebForms, e.g. LDAP://DC=halan,DC=pk (kept for parity / domain parsing).
        /// </summary>
        public string Path { get; set; } = "LDAP://DC=halan,DC=pk";

        /// <summary>
        /// NetBIOS/DNS domain name for LogonUser (e.g. halan). If empty, parsed from Path.
        /// </summary>
        public string? Domain { get; set; } = "halan";
    }
}
