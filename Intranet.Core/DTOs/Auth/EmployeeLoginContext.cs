namespace Intranet.Core.DTOs.Auth
{
    /// <summary>
    /// Auth payload equivalent to WebForms Session values after successful login.
    /// </summary>
    public class EmployeeLoginContext
    {
        public int EmpAutoId { get; set; }
        public string LoginId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Designation { get; set; }
        public int LoginHistoryId { get; set; }
    }
}
