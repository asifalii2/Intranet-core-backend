using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Intranet.Domain.Entities
{
    public class NotificationUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NotificationId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadTime { get; set; }
        public Notification Notification { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
