using Intranet.Core.Interfaces.Services;
using Intranet.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Intranet.Infrastructure.Identity
{
    public class AuthUser : IAuthUser
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }

        public AuthUser(IHttpContextAccessor httpContextAccessor, IntranetDBContext context)
        {
            var claimsIdentity = httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            if (claimsIdentity is not null)
            {
                UserId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                Email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
                Role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
            }
        }
    }
}
