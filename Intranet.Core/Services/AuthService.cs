using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Intranet.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly ISettingsService _settingsService;
        public AuthService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _settingsService.GetJWTAuthenticationSettings();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var claims = new List<Claim>
             {
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Aud, jwtSettings.ValidAudience),
                 new Claim(JwtRegisteredClaimNames.Iss, jwtSettings.ValidIssuer),
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                 new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                 new Claim(ClaimTypes.Email, user.Email),
                 new Claim(ClaimTypes.Role, user.Role),
             };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.MinutesToExpiration),
                Issuer = jwtSettings.ValidIssuer,
                Audience = jwtSettings.ValidAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a short-lived JWT that is only used for 2FA (OTP) verification.
        /// It uses a separate audience so it is NOT accepted by the normal JWT bearer authentication.
        /// </summary>
        public string GenerateTempJwtToken(User user)
        {
            var jwtSettings = _settingsService.GetJWTAuthenticationSettings();
            var securitySettings = _settingsService.GetSecuritySettings();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            // Use a distinct audience for 2FA-only tokens
            var tempAudience = $"{jwtSettings.ValidAudience}-2fa";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                // Mark this token as 2FA-only so it can be distinguished if needed
                new Claim("token_type", "2fa")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Use OTP expiration window for temp token lifetime
                Expires = DateTime.UtcNow.AddMinutes(securitySettings.OtpExpirationMinutes),
                Issuer = jwtSettings.ValidIssuer,
                Audience = tempAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
