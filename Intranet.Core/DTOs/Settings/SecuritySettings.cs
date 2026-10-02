using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Settings
{
    public class SecuritySettings
    {
        public int OtpExpirationMinutes { get; set; }
        public int OtpFailedAttemptsCount { get; set; }
        public int[] LockoutDurations { get; set; } = Array.Empty<int>();
    }
}
