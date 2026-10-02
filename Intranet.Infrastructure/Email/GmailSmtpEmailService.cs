using Intranet.Core.DTOs.Settings;
using Intranet.Core.Enums;
using Intranet.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Intranet.Infrastructure.Email
{
    public class GmailSmtpEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IEmailTemplateService _emailTemplateService;

        public GmailSmtpEmailService(ISettingsService settingsService, IEmailTemplateService emailTemplateService)
        {
            _emailSettings = settingsService.GetEmailSettings();
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendEmailAsync(EmailTemplate template, Dictionary<string, string> placeholders, List<string> toEmails, string subject)
        {
            try
            {

                var body = await _emailTemplateService.GetTemplateAsync(template, placeholders);

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.AppPassword),
                    EnableSsl = _emailSettings.SslEnabled,
                    UseDefaultCredentials = _emailSettings.UseDefaultCredentials
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.EmailDisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                // Add all recipients

                foreach (var toEmail in toEmails.Distinct())
                {
                    if (!string.IsNullOrWhiteSpace(toEmail))
                    {
                        mailMessage.To.Add(toEmail);
                    }
                }

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
