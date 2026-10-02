using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Settings
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string FromEmail { get; set; }
        public string SupportEmail { get; set; }
        public string AppPassword { get; set; }
        public bool SslEnabled { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string EmailDisplayName { get; set; }
    }
}
