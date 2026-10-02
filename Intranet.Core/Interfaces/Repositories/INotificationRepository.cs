using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<int> AddNotificationAsync(NotificationForCreation notificationForCreation);
        Task<bool> UpdateNotificationUserAsync(int id);
        Task<bool> SetAllNotificationsReadAsync();
        Task<PagedList<NotificationForListing>> GetNotificationsAsync(NotificationRequestFilter? filter);
        Task<bool> IsNotificationExistAsync(int notificationId);
        Task<int> GetUnreadNotificationCountAsync();
        Task<List<NotificationCountDto>> GetNotificationCountsAsync();
    }
}
