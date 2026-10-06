namespace Intranet.Core.DTOs.Auth
{
    /// <summary>
    /// Login API response — mirrors WebForms session profile + JWT token.
    /// </summary>
    public class LoginResponse
    {
        public int EmpAutoId { get; set; }
        public string LoginId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Designation { get; set; }
        public int LoginHistoryId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
