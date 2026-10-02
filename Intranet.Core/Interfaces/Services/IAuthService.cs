using Intranet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        /// <summary>
        /// Generates a short-lived JWT used only for 2FA (OTP) verification.
        /// This token uses a separate audience so it cannot be used as a normal access token.
        /// </summary>
        /// <param name="user">The user for whom the temp token is generated.</param>
        /// <returns>Signed JWT string.</returns>
        string GenerateTempJwtToken(User user);
    }
}
