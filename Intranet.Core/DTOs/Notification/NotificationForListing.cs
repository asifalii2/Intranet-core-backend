using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.DTOs.Notification
{
    public class NotificationForListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? EntityId { get; set; }
        public string? Entity { get; set; }
        public string? Blob { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
