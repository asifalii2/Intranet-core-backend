using Intranet.Core.Enums;
using Intranet.Core.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Infrastructure.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IHostEnvironment _env;

        public EmailTemplateService(IHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> GetTemplateAsync(
            EmailTemplate template,
            Dictionary<string, string> placeholders)
        {
            var path = Path.Combine(
                _env.ContentRootPath,
                "Templates",
                "EmailTemplates",
                $"{template}.html");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Template not found: {path}");

            var content = await File.ReadAllTextAsync(path);

            foreach (var item in placeholders)
                content = content.Replace($"{{{{{item.Key}}}}}", item.Value);

            return content;
        }
    }
}
