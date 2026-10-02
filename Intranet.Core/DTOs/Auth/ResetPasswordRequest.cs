using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Core.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain at least one capital letter, one small letter, and one number.")]

        public string NewPassword { get; set; }
    }
}
