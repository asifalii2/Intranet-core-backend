using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string title, string message, string entityType, int entityId, List<int> userIds);
        Task<bool> UpdateNotificationUserAsync(int id);
        Task<bool> SetAllNotificationsReadAsync();
        Task<PagedList<NotificationForListing>> GetNotificationsAsync(NotificationRequestFilter? filter);
        Task<bool> IsNotificationExistAsync(int notificationId);
        Task<int> GetUnreadNotificationCountAsync();
        Task<List<NotificationCountDto>> GetNotificationCountsAsync();
    }
}
