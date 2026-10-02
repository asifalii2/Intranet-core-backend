using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Intranet.Domain.Entities
{
    public class RecentActivityUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ActivityId { get; set; }
        [Required]
        public int UserId { get; set; }
        public Activity Activity { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
