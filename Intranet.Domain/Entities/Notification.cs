using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? EntityId { get; set; }
        public string? Entity { get; set; }
        public string? Blob { get; set; }
        public List<NotificationUser> NotificationUsers { get; set; }
    }
}
