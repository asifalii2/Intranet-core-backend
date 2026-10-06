using Intranet.Core.DTOs.Auth;
using Intranet.Domain.Entities;

namespace Intranet.Core.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);

        /// <summary>
        /// JWT for intranet employee login (WebForms session equivalent claims).
        /// </summary>
        string GenerateJwtToken(EmployeeLoginContext loginContext);

        /// <summary>
        /// Generates a short-lived JWT used only for 2FA (OTP) verification.
        /// This token uses a separate audience so it cannot be used as a normal access token.
        /// </summary>
        string GenerateTempJwtToken(User user);
    }
}
