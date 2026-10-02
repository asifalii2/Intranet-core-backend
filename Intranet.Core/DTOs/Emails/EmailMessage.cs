using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Emails
{
    public class EmailMessage
    {
        public List<string> ToEmails { get; set; }
        public string? EmailDisplayName { get; set; }
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }
    }
}
