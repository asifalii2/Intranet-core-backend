using Intranet.Core.DTOs.RecentActivityUser;
using Intranet.Domain.Entities;

namespace Intranet.Core.DTOs.Activities;

public class ActivityForCreation
{
    public string ActionDescription { get; set; }
    public string EntityType { get; set; } // e.g., "Job", "JobTask", "Comment"
    public int? EntityId { get; set; }
    public string? EntityName { get; set; }
    public List<RecentActivityUserForCreation> RecentActivityUsers { get; set; }
}

