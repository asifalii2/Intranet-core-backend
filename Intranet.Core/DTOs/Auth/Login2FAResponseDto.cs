using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Auth
{
    public class Login2FAResponseDto
    {
        public bool IsRequires2FA { get; set; }

        /// <summary>
        /// Available 2FA methods (Email, SMS)
        /// </summary>
        public List<string> Methods { get; set; } = new();

        /// <summary>
        /// Short-lived token used only for OTP verification
        /// </summary>
        public string TempToken { get; set; } = string.Empty;
    }

}
