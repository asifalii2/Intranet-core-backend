using Intranet.Core.Constants;
using Intranet.Core.DTOs.Activities;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.DTOs.RecentActivityUser;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;


namespace Intranet.Core.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IAttachmentRepository _attachmentRepository;

        public ActivityService(
            IActivityRepository activityRepository,
            IAttachmentRepository attachmentRepository)
        {
            _activityRepository = activityRepository;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<int> AddActivityAsync(string actionDescription, string entityType, int entityId)
        {
            string? entityName = null;
            List<RecentActivityUserForCreation> recentActivityUsers = new List<RecentActivityUserForCreation>();

            // Get entity name based on entity type
            switch (entityType)
            {
                case "Attachment":
                    var attachment = await _attachmentRepository.GetAttachmentByIdAsync(entityId, RoleConstants.SuperAdmin); // Use SuperAdmin to bypass visibility check
                    break;
            }

            var activityForCreation = new ActivityForCreation
            {
                ActionDescription = actionDescription,
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                RecentActivityUsers = recentActivityUsers
            };

            return await _activityRepository.AddActivityAsync(activityForCreation);
        }

        public async Task<int> AddActivityAsync(string actionDescription, string entityType, string entityName)
        {
            var activityForCreation = new ActivityForCreation
            {
                ActionDescription = actionDescription,
                EntityType = entityType,
                EntityId = null,
                EntityName = entityName
            };

            return await _activityRepository.AddActivityAsync(activityForCreation);
        }

        public async Task<PagedList<ActivityForListing>> GetActivitiesAsync(ActivitiesRequestFilter filter)
        {
            return await _activityRepository.GetActivitiesAsync(filter);
        }
    }
}

