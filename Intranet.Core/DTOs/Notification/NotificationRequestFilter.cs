using Intranet.Core.DTOs.Paginations;
using Intranet.Domain.Enums;

namespace Intranet.Core.DTOs.Notification
{
    public class NotificationRequestFilter : Pagination
    {
        public string? Entity { get; set; }
        public NotificationType NotificationType { get; set; } = NotificationType.All;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
