using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Notification
{
    public class NotificationMessageDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Entity { get; set; }
        public int EntityId { get; set; }
    }
}
