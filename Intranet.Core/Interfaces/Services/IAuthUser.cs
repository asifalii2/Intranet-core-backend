namespace Intranet.Core.Interfaces.Services
{
    public interface IAuthUser
    {
        /// <summary>Employee auto ID (Employee.ID) — equivalent to WebForms EmpAutoID.</summary>
        int UserId { get; }

        string LoginId { get; }
        string EmployeeId { get; }
        string Name { get; }
        string? Email { get; }
        string Role { get; }
        string? Location { get; }
        string? Designation { get; }
        int? LoginHistoryId { get; }
    }
}
