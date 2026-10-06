using Intranet.Core.DTOs.Auth;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Intranet.Core.Services
{
    public class AuthService : IAuthService
    {
        public const string ClaimLoginId = "login_id";
        public const string ClaimEmployeeId = "employee_id";
        public const string ClaimLocation = "location";
        public const string ClaimDesignation = "designation";
        public const string ClaimLoginHistoryId = "login_history_id";

        private readonly ISettingsService _settingsService;

        public AuthService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _settingsService.GetJWTAuthenticationSettings();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, jwtSettings.ValidAudience),
                new Claim(JwtRegisteredClaimNames.Iss, jwtSettings.ValidIssuer),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            return CreateToken(claims, jwtSettings.MinutesToExpiration, jwtSettings.ValidAudience);
        }

        public string GenerateJwtToken(EmployeeLoginContext loginContext)
        {
            var jwtSettings = _settingsService.GetJWTAuthenticationSettings();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, jwtSettings.ValidAudience),
                new Claim(JwtRegisteredClaimNames.Iss, jwtSettings.ValidIssuer),
                new Claim(JwtRegisteredClaimNames.Sub, loginContext.EmpAutoId.ToString()),
                new Claim(ClaimTypes.Name, loginContext.UserFullName),
                new Claim(ClaimTypes.Role, loginContext.Role ?? string.Empty),
                new Claim(ClaimLoginId, loginContext.LoginId),
                new Claim(ClaimEmployeeId, loginContext.EmployeeId),
                new Claim(ClaimLocation, loginContext.Location ?? string.Empty),
                new Claim(ClaimDesignation, loginContext.Designation ?? string.Empty),
                new Claim(ClaimLoginHistoryId, loginContext.LoginHistoryId.ToString()),
            };

            return CreateToken(claims, jwtSettings.MinutesToExpiration, jwtSettings.ValidAudience);
        }

        /// <summary>
        /// Generates a short-lived JWT that is only used for 2FA (OTP) verification.
        /// It uses a separate audience so it is NOT accepted by the normal JWT bearer authentication.
        /// </summary>
        public string GenerateTempJwtToken(User user)
        {
            var jwtSettings = _settingsService.GetJWTAuthenticationSettings();
            var securitySettings = _settingsService.GetSecuritySettings();
            var tempAudience = $"{jwtSettings.ValidAudience}-2fa";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("token_type", "2fa")
            };

            return CreateToken(claims, securitySettings.OtpExpirationMinutes, tempAudience);
        }

        private string CreateToken(IEnumerable<Claim> claims, int minutesToExpiration, string audience)
        {
            var jwtSettings = _settingsService.GetJWTAuthenticationSettings();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutesToExpiration),
                Issuer = jwtSettings.ValidIssuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
