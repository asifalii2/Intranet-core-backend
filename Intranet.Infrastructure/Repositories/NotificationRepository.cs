using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Notification;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Intranet.Domain.Enums;
using Intranet.Infrastructure.Contexts;
using Intranet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IntranetDBContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthUser _authUser;
        public NotificationRepository(IntranetDBContext context, IMapper mapper, IAuthUser authUser)
        {
            _context = context;
            _mapper = mapper;
            _authUser = authUser;
        }
        public async Task<int> AddNotificationAsync(NotificationForCreation notificationForCreation)
        {
            var notification = _mapper.Map<Notification>(notificationForCreation);

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification.Id;
        }

        public async Task<bool> UpdateNotificationUserAsync(int id)
        {
            var notificationUser = await _context.NotificationUsers
                .FirstOrDefaultAsync(x => x.NotificationId == id && !x.IsRead && x.UserId == _authUser.UserId);

            if (notificationUser == null)
                return false;

            notificationUser.IsRead = true;
            notificationUser.ReadTime = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetAllNotificationsReadAsync()
        {
            return await _context.NotificationUsers
            .Where(nu =>
                nu.UserId == _authUser.UserId &&
                !nu.IsRead &&
                !nu.Notification.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(nu => nu.IsRead, true)
                .SetProperty(nu => nu.ReadTime, DateTime.UtcNow)) > 0;
        }

        public async Task<PagedList<NotificationForListing>> GetNotificationsAsync(NotificationRequestFilter filter)
        {
            var query = _context.NotificationUsers
                .Where(x => x.UserId == _authUser.UserId && !x.Notification.IsDeleted)
                .AsNoTracking();

            if (filter.NotificationType == NotificationType.UnRead)
                query = query.Where(x => !x.IsRead);


            var query2 = query.ProjectTo<NotificationForListing>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(filter.Entity))
                query2 = query2.Where(x => x.Entity == filter.Entity);

            if (filter.FromDate.HasValue)
                query2 = query2.Where(x => x.CreatedAt >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query2 = query2.Where(x => x.CreatedAt <= filter.ToDate);

            query2 = query2.OrderByDescending(x => x.Id);

            var notifications = await query2.ToPagedListAsync(filter);

            return notifications;
        }
        public async Task<bool> IsNotificationExistAsync(int notificationId)
        {
            return await _context.Notifications.Where(x => !x.IsDeleted).AnyAsync(x => x.Id == notificationId);
        }

        public async Task<int> GetUnreadNotificationCountAsync()
        {
            return await _context.NotificationUsers
            .Where(nu =>
                nu.UserId == _authUser.UserId &&
                !nu.IsRead &&
                !nu.Notification.IsDeleted)
            .CountAsync();
        }
        public async Task<List<NotificationCountDto>> GetNotificationCountsAsync()
        {
            var entityGroup = await _context.NotificationUsers
                .Where(nu =>
                    nu.UserId == _authUser.UserId &&
                    !nu.Notification.IsDeleted)
                .GroupBy(nu => nu.Notification.Entity)
                .Select(g => new NotificationCountDto
                {
                    Entity = g.Key,
                    Count = g.Count()
                })
                .AsNoTracking()
                .ToListAsync();

            return entityGroup;
        }
    }
}
