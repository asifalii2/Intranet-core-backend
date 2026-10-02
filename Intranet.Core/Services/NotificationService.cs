using Intranet.Core.Constants;
using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.NotificationUser;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;

namespace Intranet.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAuthUser _authUser;
        private readonly INotificationHubService _notificationHubService;
        private readonly IUserRepository _userRepository;
        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationHubService notificationHubService,
            IUserRepository userRepository,
            IAuthUser authUser)
        {
            _notificationRepository = notificationRepository;
            _authUser = authUser;
            _notificationHubService = notificationHubService;
            _userRepository = userRepository;
        }

        public async Task CreateNotificationAsync(string title, string message, string entityType, int entityId, List<int> userIds)
        {

            // Send notificaion to supper admin as well
            userIds.Add(1);

            // Filter out current user from notification recipients
            var filteredUserIds = userIds.Where(id => id != _authUser.UserId).Distinct().ToList();

            if (!filteredUserIds.Any())
                return;

            var notificationForCreation = new NotificationForCreation
            {
                Title = title,
                Message = message,
                Entity = entityType,
                EntityId = entityId,
                NotificationUsers = filteredUserIds.Select(userId => new NotificationUserForCreation { UserId = userId }).ToList()
            };

            await _notificationRepository.AddNotificationAsync(notificationForCreation);

            // Real-time via SignalR when connected; Firebase push when disconnected
            var notificationMessage = new NotificationMessageDto
            {
                Title = title,
                Message = message,
                Entity = entityType,
                EntityId = entityId
            };

            await _notificationHubService.SendNotificationAsync(notificationMessage, filteredUserIds);
        }


        public async Task<int> AddNotificationAsync(NotificationForCreation notificationForCreation)
        {
            return await _notificationRepository.AddNotificationAsync(notificationForCreation);
        }

        public async Task<List<NotificationCountDto>> GetNotificationCountsAsync()
        {
            return await _notificationRepository.GetNotificationCountsAsync();
        }

        public async Task<PagedList<NotificationForListing>> GetNotificationsAsync(NotificationRequestFilter? filter)
        {
            return await _notificationRepository.GetNotificationsAsync(filter);
        }

        public async Task<int> GetUnreadNotificationCountAsync()
        {
            return await _notificationRepository.GetUnreadNotificationCountAsync();
        }

        public async Task<bool> IsNotificationExistAsync(int notificationId)
        {
            return await _notificationRepository.IsNotificationExistAsync(notificationId);
        }

        public async Task<bool> SetAllNotificationsReadAsync()
        {
            return await _notificationRepository.SetAllNotificationsReadAsync();
        }

        public async Task<bool> UpdateNotificationUserAsync(int id)
        {
            return await _notificationRepository.UpdateNotificationUserAsync(id);
        }
    }
}
