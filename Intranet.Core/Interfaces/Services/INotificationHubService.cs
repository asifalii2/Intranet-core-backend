using Intranet.Core.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Services
{
    public interface INotificationHubService
    {
        Task SendNotificationAsync(NotificationMessageDto message, List<int> userIds);
    }
}
