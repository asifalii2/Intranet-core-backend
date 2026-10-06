namespace Intranet.Core.Interfaces.Services
{
    public interface ILdapAuthService
    {
        /// <summary>
        /// Authenticates against Active Directory using the same LDAP bind approach as WebForms.
        /// </summary>
        bool Authenticate(string username, string password);
    }
}
