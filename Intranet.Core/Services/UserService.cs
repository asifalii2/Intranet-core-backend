using Intranet.Core.DTOs.Paginations;
using Intranet.Core.DTOs.Users;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;

namespace Intranet.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public  async Task<int> AddUserAsync(UserForCreation userForCreation)
        {
            return await _userRepository.AddUserAsync(userForCreation);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<string?> GetUserRoleByIdAsync(int id)
        {
            return await _userRepository.GetUserRoleByIdAsync(id);
        }

        public async Task<PagedList<User>> GetUsersAsync(UsersRequestFilter? filter)
        {
            return await _userRepository.GetUsersAsync(filter);
        }

        public async Task<bool> IsUserEmailExistAsync(string email, int id = 0)
        {
            return await _userRepository.IsUserEmailExistAsync(email, id);
        }

        public async Task<bool> IsUserExistAsync(int id)
        {
            return await _userRepository.IsUserExistAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            return await _userRepository.SoftDeleteUserAsync(id);
        }

        public async Task<bool> UpdateUserAsync(UserForUpdate userForUpdation)
        {
            return await _userRepository.UpdateUserAsync(userForUpdation);
        }

        public async Task<bool> UpdateUserStatusAsync(UserStatusForUpdate userStatusForUpdate)
        {
            return await _userRepository.UpdateUserStatusAsync(userStatusForUpdate);
        }

        public async Task<bool> UpdateUserEmailTwoFactorEnabledStatusAsync(UserEmailTwoFactorEnabledStatusForUpdate userEmailTwoFactorEnabledStatusForUpdate)
        {
            return await _userRepository.UpdateUserEmailTwoFactorEnabledStatusAsync(userEmailTwoFactorEnabledStatusForUpdate);
        }

        public async Task<bool> UpdateUserPhoneTwoFactorEnabledStatusAsync(UserPhoneTwoFactorEnabledStatusForUpdate userPhoneTwoFactorEnabledStatusForUpdate)
        {
            return await _userRepository.UpdateUserPhoneTwoFactorEnabledStatusAsync(userPhoneTwoFactorEnabledStatusForUpdate);
        }

        public async Task<bool> UpdateUsersStatusBulkAsync(UsersStatusForBulkUpdate bulkUserStatusForUpdate)
        {
            return await _userRepository.UpdateUsersStatusBulkAsync(bulkUserStatusForUpdate);
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> ids)
        {
            return await _userRepository.GetUsersByIdsAsync(ids);
        }

        public async Task<bool> SoftDeleteBulkUsersAsync(List<int> userIds)
        {
           return await _userRepository.SoftDeleteBulkUsersAsync(userIds);
        }
    }
}
