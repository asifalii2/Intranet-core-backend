using Intranet.Domain.Entities;

namespace Intranet.Core.Interfaces.Repositories
{
    public interface ILoginRepository
    {
        Task UpdateLastLoginDateAsync(string loginId, DateTime lastLoginDate);
        Task<LoginHistory> AddLoginHistoryAsync(LoginHistory loginHistory);
        Task SaveChangesAsync();
    }
}
