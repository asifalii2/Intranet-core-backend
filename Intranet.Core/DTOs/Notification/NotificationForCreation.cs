using Intranet.Core.DTOs.NotificationUser;
using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Notification
{
    public class NotificationForCreation
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        public int? EntityId { get; set; }
        public string? Entity { get; set; }
        public string? Blob { get; set; }
        public List<NotificationUserForCreation> NotificationUsers { get; set; }
    }
}
