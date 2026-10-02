using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Auth
{
    public class VerifyOtpRequestDto
    {
        [Required]
        public string TempToken { get; set; } = string.Empty;

        /// <summary>
        /// OTP sent via email (required if email 2FA is enabled).
        /// </summary>
        public int? EmailOtp { get; set; }

        /// <summary>
        /// OTP sent via phone/SMS (required if phone 2FA is enabled).
        /// </summary>
        public int? PhoneOtp { get; set; }
    }
}
