using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Settings
{
    public class JWTAuthenticationSettings
    {
        public string SecretKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int MinutesToExpiration { get; set; }
        public string? MasterPassword { get; set; }
    }
}
