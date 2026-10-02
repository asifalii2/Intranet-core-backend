using Intranet.Core.DTOs.Emails;
using Intranet.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailTemplate template, Dictionary<string, string> placeholders, List<string> toEmails, string subject);

    }
}
