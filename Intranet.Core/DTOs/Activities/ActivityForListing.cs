using Intranet.Core.DTOs.Users;

namespace Intranet.Core.DTOs.Activities;

public class ActivityForListing
{
    public int Id { get; set; }
    public string ActionDescription { get; set; }
    public string EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? EntityName { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserOverviewDto User { get; set; }
}

