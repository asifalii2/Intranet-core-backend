using Intranet.Core.DTOs.Notification;
using Intranet.Core.Interfaces.Services;
using Intranet.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Intranet.Services
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public NotificationHubService(
            IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(NotificationMessageDto message, List<int> userIds)
        {
            var connectedUserIds = new List<int>();
            var disconnectedUserIds = new List<int>();

            foreach (var userId in userIds.Distinct())
            {
                if (SignalRHub.IsUserConnected(userId))
                    connectedUserIds.Add(userId);
                else
                    disconnectedUserIds.Add(userId);
            }

            var tasks = new List<Task>();

            if (connectedUserIds.Count > 0)
                tasks.Add(SendSignalRNotificationAsync(message, connectedUserIds));

            await Task.WhenAll(tasks);
        }

        private async Task SendSignalRNotificationAsync(NotificationMessageDto message, List<int> userIds)
        {
            var tasks = userIds.Select(async userId =>
            {
                var userConnections = SignalRHub._connections
                    .Where(c => c.UserId == userId)
                    .Select(c => c.ConnectionId)
                    .ToList();

                if (userConnections.Count > 0)
                {
                    await _hubContext.Clients
                        .Clients(userConnections)
                        .SendAsync("ReceiveNotification", message);
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
