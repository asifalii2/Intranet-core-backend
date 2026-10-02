using Intranet.Core.DTOs.Paginations;
using Intranet.Core.DTOs.Users;
using Intranet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<int> AddUserAsync(UserForCreation userForCreation);
        Task<bool> DeleteUserAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<string?> GetUserRoleByIdAsync(int id);
        Task<PagedList<User>> GetUsersAsync(UsersRequestFilter? filter);
        Task<bool> IsUserEmailExistAsync(string email, int id = 0);
        Task<bool> IsUserExistAsync(int id);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> UpdateUserAsync(UserForUpdate userForUpdation);
        Task<bool> UpdateUserStatusAsync(UserStatusForUpdate userStatusForUpdate);
        Task<bool> UpdateUserEmailTwoFactorEnabledStatusAsync(UserEmailTwoFactorEnabledStatusForUpdate userEmailTwoFactorEnabledStatusForUpdate);
        Task<bool> UpdateUserPhoneTwoFactorEnabledStatusAsync(UserPhoneTwoFactorEnabledStatusForUpdate userPhoneTwoFactorEnabledStatusForUpdate);
        Task<bool> UpdateUsersStatusBulkAsync(UsersStatusForBulkUpdate bulkUserStatusForUpdate);
        Task<List<User>> GetUsersByIdsAsync(List<int> ids);
        Task<bool> SoftDeleteBulkUsersAsync(List<int> userIds);
        Task<int> SaveChangesAsync();
    }
}
