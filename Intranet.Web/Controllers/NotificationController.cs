using Intranet.Core.Constants;
using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.Interfaces.Services;
using Intranet.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthUser _authUser;
        
        public NotificationController(INotificationService notificationService, IAuthUser authUser)
        {
            _notificationService = notificationService;
            _authUser = authUser;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationForListing>>> GetNotificationsAsync([FromQuery] NotificationRequestFilter filter)
        {
            var notifications = await _notificationService.GetNotificationsAsync(filter);

            Response.AddPaginationHeader(notifications);

            return Ok(notifications);
        }

        [HttpGet("notification-count")]
        public async Task<ActionResult<List<NotificationCountDto>>> GetNotificationCountsAsync()
        {
            var notifications = await _notificationService.GetNotificationCountsAsync();

            return Ok(notifications);
        }

        [HttpGet("unread-notification-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationCountAsync()
        {
            var unreadCount = await _notificationService.GetUnreadNotificationCountAsync();

            return Ok(unreadCount);
        }

        [HttpPut("{notificationId}/mark-as-read")]
        public async Task<ActionResult> SetAsRead([FromRoute] int notificationId)
        {
            var notificationUser = await _notificationService.UpdateNotificationUserAsync(notificationId);
            if (!notificationUser)
                return BadRequest(new { Message = "This notification has already been marked as read." });

            return Ok(new { Message = $"Notification marked as read successfully." });
        }

        [HttpPut("mark-all-as-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var markedNotifications = await _notificationService.SetAllNotificationsReadAsync();
            if (!markedNotifications)
                return BadRequest(new { Message = "No unread notifications to mark as read." });

            return Ok(new { Message = $"Notifications marked as read successfully." });
        }
    }
}
