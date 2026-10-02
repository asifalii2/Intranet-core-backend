using Intranet.Core.DTOs.Activities;
using Intranet.Core.DTOs.Paginations;
using Intranet.Domain.Entities;

namespace Intranet.Core.Interfaces.Repositories;

public interface IActivityRepository
{
    Task<int> AddActivityAsync(ActivityForCreation activityForCreation);
    Task<PagedList<ActivityForListing>> GetActivitiesAsync(ActivitiesRequestFilter filter);
}

