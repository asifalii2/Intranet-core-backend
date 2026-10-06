using Intranet.Core.Interfaces.Services;
using Intranet.Core.Services;
using Intranet.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Intranet.Infrastructure.Identity
{
    public class AuthUser : IAuthUser
    {
        public int UserId { get; private set; }
        public string LoginId { get; private set; } = string.Empty;
        public string EmployeeId { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        public string Role { get; private set; } = string.Empty;
        public string? Location { get; private set; }
        public string? Designation { get; private set; }
        public int? LoginHistoryId { get; private set; }

        public AuthUser(IHttpContextAccessor httpContextAccessor, IntranetDBContext context)
        {
            var claimsIdentity = httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            if (claimsIdentity is null)
                return;

            if (int.TryParse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                UserId = userId;

            Name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            Email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            Role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            LoginId = claimsIdentity.FindFirst(AuthService.ClaimLoginId)?.Value ?? string.Empty;
            EmployeeId = claimsIdentity.FindFirst(AuthService.ClaimEmployeeId)?.Value ?? string.Empty;
            Location = claimsIdentity.FindFirst(AuthService.ClaimLocation)?.Value;
            Designation = claimsIdentity.FindFirst(AuthService.ClaimDesignation)?.Value;

            if (int.TryParse(claimsIdentity.FindFirst(AuthService.ClaimLoginHistoryId)?.Value, out var historyId))
                LoginHistoryId = historyId;
        }
    }
}
