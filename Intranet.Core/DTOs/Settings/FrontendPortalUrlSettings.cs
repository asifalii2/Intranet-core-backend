using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Settings
{
    public class FrontendPortalUrlSettings
    {
        public string BaseUrl { get; set; }
        public string UserPasswordResetUrl { get; set; }
        public string LoginUrl { get; set; }
    }
}
