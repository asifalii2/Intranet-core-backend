using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Core.DTOs.NotificationUser
{
    public class NotificationUserForCreation
    {
        [Required]
        public int UserId { get; set; }
    }
}
