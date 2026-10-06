using Intranet.Core.Interfaces.Repositories;
using Intranet.Domain.Entities;
using Intranet.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Infrastructure.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IntranetDBContext _context;

        public LoginRepository(IntranetDBContext context)
        {
            _context = context;
        }

        public async Task UpdateLastLoginDateAsync(string loginId, DateTime lastLoginDate)
        {
            var login = await _context.Logins
                .FirstOrDefaultAsync(l => l.LoginId == loginId);

            if (login == null)
                return;

            login.LastLoginDate = lastLoginDate;
        }

        public async Task<LoginHistory> AddLoginHistoryAsync(LoginHistory loginHistory)
        {
            await _context.LoginHistories.AddAsync(loginHistory);
            await _context.SaveChangesAsync();
            return loginHistory;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
