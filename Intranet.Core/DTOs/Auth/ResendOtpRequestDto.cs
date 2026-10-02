using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Auth
{
    public class ResendOtpRequestDto
    {
        /// <summary>
        /// Short-lived temp JWT returned from the login (2FA step 1).
        /// </summary>
        [Required]
        public string TempToken { get; set; } = string.Empty;
    }
}
