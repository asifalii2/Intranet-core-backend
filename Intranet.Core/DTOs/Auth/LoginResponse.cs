using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Auth
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public int? AvatarAttachmentId { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
