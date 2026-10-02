using Intranet.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        Task<string> GetTemplateAsync(
            EmailTemplate template,
            Dictionary<string, string> placeholders);
    }
}
