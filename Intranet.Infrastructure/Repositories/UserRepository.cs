using AutoMapper;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.DTOs.Users;
using Intranet.Core.Helpers;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Intranet.Domain.Enums;
using Intranet.Infrastructure.Contexts;
using Intranet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IntranetDBContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthUser _authUser;
        public UserRepository(IntranetDBContext context, IMapper mapper, IAuthUser authUser)
        {
            _context = context;
            _mapper = mapper;
            _authUser = authUser;
        }
        public async Task<int> AddUserAsync(UserForCreation userForCreation)
        {
            var user = _mapper.Map<User>(userForCreation);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id == 1)
                return false;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;

        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.Email == email);
            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return user;
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> ids)
        {
            return await _context.Users
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<string?> GetUserRoleByIdAsync(int id)
        {
            var role = await _context.Users
                 .Where(x => x.Id == id && !x.IsDeleted && x.IsActive)
                 .Select(x => x.Role)
                 .FirstOrDefaultAsync();

            return role;
        }

        public async Task<PagedList<User>> GetUsersAsync(UsersRequestFilter? filter)
        {
            var query = _context.Users.Where(x =>  !x.IsDeleted)
                .AsNoTracking();
    
            if (filter.IsActive.HasValue)
                query = query.Where(x => x.IsActive == filter.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(x =>
                (x.FirstName + " " + x.LastName).Contains(filter.Search) ||
                x.Email.Contains(filter.Search) ||
                x.StreetAddress.Contains(filter.Search) ||
                x.City.Contains(filter.Search) ||
                x.Suburb.Contains(filter.Search) ||
                x.PostalCode.Contains(filter.Search) ||
                x.CompanyName.Contains(filter.Search)
                );

            query = query.OrderByDescending(x => x.Id);

            var result = await query.ToPagedListAsync(filter);

            return result;
        }

        public async Task<bool> IsUserEmailExistAsync(string email, int id = 0)
        {
            return await _context.Users.AnyAsync(x => x.Email == email && x.Id != id);
        }

        public async Task<bool> IsUserExistAsync(int id)
        {
            return await _context.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> SoftDeleteBulkUsersAsync(List<int> userIds)
        {
            var affectedRows = await _context.Users
                   .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
                   .ExecuteUpdateAsync(u => u
                       .SetProperty(x => x.IsDeleted, true)
                       .SetProperty(x => x.DeletedAt, DateTime.UtcNow)
                       .SetProperty(x => x.DeletedById, _authUser.UserId)
                       .SetProperty(x => x.Email, x => "DELETED_" + x.Email + "_" + UniqueNumberGenerator.Generate())
                   );

            return affectedRows > 0;
        }

        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            if (id == 1)
                return false;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return false;

            user.Email = $"DELETED_{user.Email}_{UniqueNumberGenerator.Generate()}";
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedById = _authUser.UserId;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateUserAsync(UserForUpdate userForUpdation)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userForUpdation.Id);
            if (user == null)
                return false;
            _mapper.Map(userForUpdation, user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserStatusAsync(UserStatusForUpdate userStatusForUpdate)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userStatusForUpdate.Id && !x.IsDeleted);

            if (user == null)
                return false;

            _mapper.Map(userStatusForUpdate, user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUsersStatusBulkAsync(UsersStatusForBulkUpdate bulkUserStatusForUpdate)
        {
            var affectedRows = await _context.Users
                .Where(x => bulkUserStatusForUpdate.Ids.Contains(x.Id) && !x.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsActive, bulkUserStatusForUpdate.IsActive)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                    .SetProperty(x => x.UpdatedById, _authUser.UserId));

            return affectedRows > 0;
        }

        public async Task<bool> UpdateUserEmailTwoFactorEnabledStatusAsync
            (UserEmailTwoFactorEnabledStatusForUpdate userEmailTwoFactorEnabledStatusForUpdate)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == _authUser.UserId && !x.IsDeleted);

            if (user == null)
                return false;

            _mapper.Map(userEmailTwoFactorEnabledStatusForUpdate, user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserPhoneTwoFactorEnabledStatusAsync
            (UserPhoneTwoFactorEnabledStatusForUpdate userPhoneTwoFactorEnabledStatusForUpdate)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == _authUser.UserId && !x.IsDeleted);

            if (user == null)
                return false;

            _mapper.Map(userPhoneTwoFactorEnabledStatusForUpdate, user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
