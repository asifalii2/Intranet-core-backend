using Intranet.Core.DTOs.Paginations;

namespace Intranet.Core.DTOs.Activities;

public class ActivitiesRequestFilter : Pagination
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Search { get; set; }
}

