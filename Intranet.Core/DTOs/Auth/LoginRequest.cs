using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Auth
{
    public class LoginRequest
    {
        /// <summary>
        /// AD/Global username. Email local-part is accepted (e.g. user@halan.com.pk → user).
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string Password { get; set; } = string.Empty;
    }
}
