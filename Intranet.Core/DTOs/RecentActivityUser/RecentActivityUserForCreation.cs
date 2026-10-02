using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Core.DTOs.RecentActivityUser
{
    public class RecentActivityUserForCreation
    {
        [Required]
        public int UserId { get; set; }
    }
}
