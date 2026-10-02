using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Settings
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string FromPhoneNumber { get; set; } = string.Empty;
    }
}
