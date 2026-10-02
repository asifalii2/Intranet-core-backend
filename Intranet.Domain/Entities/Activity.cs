using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intranet.Domain.Entities
{
    public class Activity : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string ActionDescription { get; set; } // e.g., "updated Property Acquisition task", "added a comment on 100 North Street Berry"
        public string EntityType { get; set; } // e.g., "Job", "JobTask", "Comment"
        public int? EntityId { get; set; } // ID of the related entity
        public string? EntityName { get; set; } // e.g., "100 North Street Berry" for Job, "Property Acquisition" for JobTask
        public List<RecentActivityUser> RecentActivityUsers { get; set; }
    }
}
