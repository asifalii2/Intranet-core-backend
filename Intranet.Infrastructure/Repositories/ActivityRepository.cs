using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Activities;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Intranet.Infrastructure.Contexts;
using Intranet.Infrastructure.Extensions;
using Intranet.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Intranet.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IntranetDBContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthUser _authUser;

        public ActivityRepository(
            IntranetDBContext context,
            IMapper mapper,
            IAuthUser authUser)
        {
            _context = context;
            _mapper = mapper;
            _authUser = authUser;
        }

        public async Task<int> AddActivityAsync(ActivityForCreation activityForCreation)
        {
            var activity = _mapper.Map<Activity>(activityForCreation);
            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
            return activity.Id;
        }

        public async Task<PagedList<ActivityForListing>> GetActivitiesAsync(ActivitiesRequestFilter filter)
        {
            IQueryable<ActivityForListing> query;

            if (_authUser.Role == RoleConstants.SuperAdmin)
            {
                query = _context.Activities
                    .Include(x => x.CreatedBy)
                    .Where(x => !x.IsDeleted)
                    .AsNoTracking()
                    .ProjectTo<ActivityForListing>(_mapper.ConfigurationProvider);
            }
            else
            {
                query = _context.RecentActivityUsers
                    .Include(x => x.User)
                    .Include(x => x.Activity)
                    .ThenInclude(a => a.CreatedBy)
                    .Where(x => !x.Activity.IsDeleted && !x.User.IsDeleted && x.UserId == _authUser.UserId)
                    .AsNoTracking()
                    .ProjectTo<ActivityForListing>(_mapper.ConfigurationProvider);
            }

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
            {
                var toDateEnd = filter.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedAt <= toDateEnd);
            }

            if (!string.IsNullOrWhiteSpace(filter.EntityType))
                query = query.Where(x => x.EntityType == filter.EntityType);

            if (filter.EntityId.HasValue)
                query = query.Where(x => x.EntityId == filter.EntityId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(x =>
                    x.ActionDescription.Contains(filter.Search) ||
                    (x.EntityName != null && x.EntityName.Contains(filter.Search))
                );

            query = query.OrderByDescending(x => x.CreatedAt);

            return await query.ToPagedListAsync(filter);
        }
    }
}

