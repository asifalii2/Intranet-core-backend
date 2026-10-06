using Intranet.Core.Interfaces.Repositories;
using Intranet.Domain.Entities;
using Intranet.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IntranetDBContext _context;

        public EmployeeRepository(IntranetDBContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByGlobalIdAsync(string globalId)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.GlobalId == globalId);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
