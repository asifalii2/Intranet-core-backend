using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Core.DTOs.Auth
{
    public class ForgotUserPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
