using Intranet.Domain.Entities;

namespace Intranet.Core.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByGlobalIdAsync(string globalId);
        Task<Employee?> GetByIdAsync(int id);
    }
}
