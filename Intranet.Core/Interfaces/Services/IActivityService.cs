using Intranet.Core.DTOs.Activities;
using Intranet.Core.DTOs.Paginations;
using Intranet.Domain.Entities;

namespace Intranet.Core.Interfaces.Services;

public interface IActivityService
{
    Task<int> AddActivityAsync(string actionDescription, string entityType, int entityId);
    Task<int> AddActivityAsync(string actionDescription, string entityType, string entityName);
    Task<PagedList<ActivityForListing>> GetActivitiesAsync(ActivitiesRequestFilter filter);
}

