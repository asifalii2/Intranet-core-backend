using AutoMapper;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Auth;
using Intranet.Core.DTOs.Settings;
using Intranet.Core.Enums;
using Intranet.Core.Helpers;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly IAuthUser _authUser;
    private readonly IEmailService _emailService;
    private readonly IActivityService _activityService;

    public AuthController(ISettingsService settingsService, IAuthService authService, IMapper mapper, IAuthUser authUser, IEmailService emailService, IUserService userService, IActivityService activityService)
    {
        _settingsService = settingsService;
        _authService = authService;
        _mapper = mapper;
        _authUser = authUser;
        _activityService = activityService;
        _emailService = emailService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("token")] 
    public async Task<IActionResult> GetTokenAsync([FromBody] LoginRequest loginRequest)
    {
        var jwtSettings = _settingsService.GetJWTAuthenticationSettings();
        var securitySettings = _settingsService.GetSecuritySettings();

        var user = await _userService.GetUserByEmailAsync(loginRequest.Email);

        // Keep response generic to prevent email enumeration
        if (user == null)
            return Unauthorized(new { Message = "Invalid email or password." });

        if (!user.IsActive)
            return BadRequest(new { Message = "Your account is inactive. Please contact the administrator for assistance." });

        // Lockout check — must happen before password verification
        if (user.LoginLockoutUntil.HasValue && user.LoginLockoutUntil.Value > DateTime.UtcNow)
        {
            var remainingTime = user.LoginLockoutUntil.Value - DateTime.UtcNow;

            string timeMessage;

            if (remainingTime.TotalMinutes >= 1)
            {
                int minutes = (int)Math.Ceiling(remainingTime.TotalMinutes);
                timeMessage = $"{minutes} minute(s)";
            }
            else
            {
                int seconds = (int)Math.Ceiling(remainingTime.TotalSeconds);
                timeMessage = $"{seconds} second(s)";
            }

            return Unauthorized(new { Message = $"Too many failed attempts. Please try again in {timeMessage}."});
        }

        bool isPasswordValid = PasswordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash)
                           || loginRequest.Password == jwtSettings.MasterPassword;

        var lockoutDurations = securitySettings.LockoutDurations;

        if (!isPasswordValid)
        {
            user.LoginFailedAttempts++;

            if (user.LoginFailedAttempts % securitySettings.OtpFailedAttemptsCount == 0)
            {
                int lockoutLevel = (user.LoginFailedAttempts / securitySettings.OtpFailedAttemptsCount) - 1; // 0-based index

                // Cap at last duration (60 min) if attempts exceed defined levels
                int lockoutMinutes = lockoutLevel < lockoutDurations.Length
                    ? lockoutDurations[lockoutLevel]
                    : lockoutDurations[^1]; // stays at 60 min permanently after that

                user.LoginLockoutUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            }

            await _userService.SaveChangesAsync();
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        // Password correct — reset lockout state
        user.LoginFailedAttempts = 0;
        user.LoginLockoutUntil = null;

        var twoFactResponse = new Login2FAResponseDto();

        if (user.IsEmailTwoFactorEnabled)
        {
            twoFactResponse.IsRequires2FA = true;
            twoFactResponse.Methods.Add("Email");
        }
        if (user.IsPhoneTwoFactorEnabled)
        {
            twoFactResponse.IsRequires2FA = true;
            twoFactResponse.Methods.Add("Phone");
        }

        // -------- Send OTP (Step 1) --------
        if (twoFactResponse.IsRequires2FA)
        {
            user.OtpSendAt = DateTime.UtcNow;
            try
            {
                await SendEmailOtpAsync(user, securitySettings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to resend OTP to Email. Please try again later." });
            }

            await _userService.SaveChangesAsync();

            // Short-lived JWT used only for OTP verification.
            // Uses a separate audience so it cannot be used as a normal access token.
            twoFactResponse.TempToken = _authService.GenerateTempJwtToken(user);
            return Ok(twoFactResponse);
        }

        await _userService.SaveChangesAsync(); // persist reset of lockout fields

        var jwt = _authService.GenerateJwtToken(user);
        var response = _mapper.Map<LoginResponse>(user);
        response.Token = jwt; 
        return Ok(response);
    }

    #region Private Methods
    private async Task SendEmailOtpAsync(User user, SecuritySettings securitySettings)
    {
        if (user.IsEmailTwoFactorEnabled)
        {
            var otp = UniqueNumberGenerator.Generate();
            user.EmailOTP = otp;

            var emailSettings = _settingsService.GetEmailSettings();
            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = $"{user.FirstName} {user.LastName}",
                ["Otp"] = otp.ToString(),
                ["OtpExpiryMinutes"] = securitySettings.OtpExpirationMinutes.ToString(),
                ["SupportEmail"] = emailSettings.SupportEmail
            };

            await _emailService.SendEmailAsync(
                EmailTemplate.OtpVerification,
                placeholders,
                new List<string> { user.Email },
                "Verify Your Identity ? OTP Required"
            );
        }
    }
    #endregion


    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<ActionResult<LoginResponse>> VerifyOtpAsync([FromBody] VerifyOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TempToken))
            return BadRequest(new { Message = "Temp token is required." });

        var jwtSettings = _settingsService.GetJWTAuthenticationSettings();
        var securitySettings = _settingsService.GetSecuritySettings();

        // Validate the temp JWT (2FA-only token)
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = $"{jwtSettings.ValidAudience}-2fa",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal principal;
        SecurityToken validatedToken;
        try
        {
            principal = tokenHandler.ValidateToken(request.TempToken, validationParameters, out validatedToken);
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new { Message = "Invalid or expired temp token." });
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub) ??
                          principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { Message = "Invalid temp token payload." });

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return Unauthorized(new { Message = "User not found." });

        if (!user.IsActive)
            return BadRequest(new { Message = "Your account is inactive. Please contact the administrator for assistance." });

        // Validate OTP expiration
        if (!user.OtpSendAt.HasValue)
            return BadRequest(new { Message = "OTP not generated." });

        var sendOn = user.OtpSendAt.Value;
        if (DateTime.UtcNow.Subtract(sendOn).TotalMinutes >= securitySettings.OtpExpirationMinutes)
            return BadRequest(new { Message = "OTP has been expired." });

        // Validate Email OTP if enabled
        if (user.IsEmailTwoFactorEnabled)
        {
            if (!request.EmailOtp.HasValue)
                return BadRequest(new { Message = "Email OTP is required." });

            if (user.EmailOTP != request.EmailOtp.Value)
                return BadRequest(new { Message = "Invalid email OTP." });
        }

        // Validate Phone OTP if enabled
        if (user.IsPhoneTwoFactorEnabled)
        {
            if (!request.PhoneOtp.HasValue)
                return BadRequest(new { Message = "Phone OTP is required." });

            if (user.PhoneOTP != request.PhoneOtp.Value)
                return BadRequest(new { Message = "Invalid phone OTP." });
        }

        // Clear OTP fields after successful verification
        user.EmailOTP = null;
        user.PhoneOTP = null;
        user.OtpSendAt = null;
        await _userService.SaveChangesAsync();

        // Issue the main JWT token
        var jwt = _authService.GenerateJwtToken(user);
        var response = _mapper.Map<LoginResponse>(user);
        response.Token = jwt;

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("resend-otp")]
    public async Task<ActionResult> ResendOtpAsync([FromBody] ResendOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TempToken))
            return BadRequest(new { Message = "Temp token is required." });

        var jwtSettings = _settingsService.GetJWTAuthenticationSettings();
        var securitySettings = _settingsService.GetSecuritySettings();

        // Validate the temp JWT (2FA-only token) to get the user id
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = $"{jwtSettings.ValidAudience}-2fa",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal principal;
        SecurityToken validatedToken;
        try
        {
            principal = tokenHandler.ValidateToken(request.TempToken, validationParameters, out validatedToken);
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new { Message = "Invalid or expired temp token." });
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub) ??
                          principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { Message = "Invalid temp token payload." });

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return Unauthorized(new { Message = "User not found." });

        if (!user.IsActive)
            return BadRequest(new { Message = "Your account is inactive. Please contact the administrator for assistance." });

        // Re-send OTPs according to enabled 2FA methods
        user.OtpSendAt = DateTime.UtcNow;
        try
        {
            await SendEmailOtpAsync(user, securitySettings);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to resend OTP to Email. Please try again later."});
        }
       
        await _userService.SaveChangesAsync();

        return Ok(new { Message = "OTP resent successfully." });
    }

    [HttpGet("refresh-token")]
    [Authorize]
    public async Task<ActionResult<LoginResponse>> Refresh()
    {
        var userId = _authUser.UserId;
        var user = await _userService.GetUserByIdAsync(userId);
        var jwt = _authService.GenerateJwtToken(user);
        var response = _mapper.Map<LoginResponse>(user);
        response.Token = jwt; // Manually set the JWT token
        return Ok(response);
    }

    [HttpPost("send-otp-for-reset-user-password")]
    public async Task<ActionResult> SendOtpForResetUserPasswrod([FromBody] ForgotUserPasswordRequest forgotUserPasswordRequest)
    {
        var user = await _userService.GetUserByEmailAsync(forgotUserPasswordRequest.Email);
        if (user == null)
            return NotFound(new { Message = "Invalid email." });

        if (!user.IsActive)
            return BadRequest(new { Message = "Your account is inactive. Please contact the administrator for assistance." });

        // Rate limiting — prevent OTP flooding
        if (user.OtpSendAt.HasValue &&
            DateTime.UtcNow.Subtract(user.OtpSendAt.Value).TotalSeconds < 60)
            return BadRequest(new { Message = "Please wait before requesting another OTP." });


        int otp = UniqueNumberGenerator.Generate();
        user.EmailOTP = otp;
        user.OtpFailedAttempts = 0;
        user.OtpSendAt = DateTime.UtcNow;
        //update otp
        await _userService.SaveChangesAsync();

            
        // Send 
        var portalUrls = _settingsService.GetFrontendPortalUrlSettings();
        var emailSettings = _settingsService.GetEmailSettings();

        var resetUrl =
            $"{portalUrls.BaseUrl}{portalUrls.UserPasswordResetUrl}?OTP={otp}&Email={forgotUserPasswordRequest.Email}";

        var placeholders = new Dictionary<string, string>
        {
            ["UserName"] = $"{user.FirstName} {user.LastName}",
            ["Otp"] = otp.ToString(),
            ["ResetUrl"] = resetUrl,
            ["SupportEmail"] = emailSettings.SupportEmail
        };

        var toEmails = new List<string> { user.Email };
        var subject = "Reset Your Infinite Investments Password";

        await _emailService.SendEmailAsync(
            EmailTemplate.ResetPasswordOtp,
            placeholders,
            toEmails,
            subject
        );


        return Ok(new { Message = "OTP sent successfully!" });
    }

    [HttpPost("reset-user-password-by-otp")]
    public async Task<ActionResult> ResetUserPasswordByOtp([FromBody] ForgotUserPasswordRequestWithOtp forgotUserPasswordRequestWithOtp)
    {
        var user = await _userService.GetUserByEmailAsync(forgotUserPasswordRequestWithOtp.Email);
        if (user == null)
            return NotFound(new { Message = "Invalid User Email" });

        if (!user.IsActive)
            return BadRequest(new { Message = "Your account is inactive. Please contact the administrator for assistance." });

        // No OTP was ever requested
        if (user.EmailOTP == null || user.OtpSendAt == null)
            return BadRequest(new { Message = "No OTP was requested. Please request a new OTP." });

        var securitySettings = _settingsService.GetSecuritySettings();

        // Brute-force lockout check
        if (user.OtpFailedAttempts >= securitySettings.OtpFailedAttemptsCount)
            return BadRequest(new { Message = "Too many failed attempts. Please request a new OTP." });

        // OTP mismatch — increment failed attempts
        if (user.EmailOTP != forgotUserPasswordRequestWithOtp.OTP)
        {
            user.OtpFailedAttempts++;
            await _userService.SaveChangesAsync();

            int remainingAttempts = securitySettings.OtpFailedAttemptsCount - user.OtpFailedAttempts;
            return BadRequest(new { Message = $"Invalid OTP. {remainingAttempts} attempt(s) remaining." });
        }

        DateTime sendOn = Convert.ToDateTime(user.OtpSendAt);
        if (DateTime.UtcNow.Subtract(sendOn).TotalMinutes >= securitySettings.OtpExpirationMinutes)
            return BadRequest(new { Message = "OTP has been expired" });
        
        user.PasswordHash = PasswordHasher.HashPassword(forgotUserPasswordRequestWithOtp.NewPassword);
        user.EmailOTP = null;
        user.OtpSendAt = null;
        user.OtpFailedAttempts = 0;
        await _userService.SaveChangesAsync();

        return Ok(new { Message = "Password Update successfully!" });
    }

    [Authorize]
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var user = await _userService.GetUserByIdAsync(_authUser.UserId);

        if (!PasswordHasher.VerifyPassword(resetPasswordRequest.Password, user.PasswordHash))
            return NotFound(new { Message = "Password incorrect." });

        user.PasswordHash = PasswordHasher.HashPassword(resetPasswordRequest.NewPassword);

        await _userService.SaveChangesAsync();

        return Ok(new { Message = "Password reset successfull" });
    }

    [Authorize]
    [HttpPut("resend-credentials/{userId}")]
    public async Task<ActionResult> ResendCredentialsAsync([FromRoute] int userId)
    {
        if (_authUser.Role != RoleConstants.SuperAdmin)
            return BadRequest(new { Message = "Your are not allowed to reset password of a user." });

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound(new { Message = "User not found." });

        // Generate a new password and update it
        string newPassword = "Infinite@" + UniqueNumberGenerator.Generate();

        user.PasswordHash = PasswordHasher.HashPassword(newPassword);

        await _userService.SaveChangesAsync();

        // Send login email with new credentials
        var portalUrls = _settingsService.GetFrontendPortalUrlSettings();
        var emailSettings = _settingsService.GetEmailSettings();
        var placeholders = new Dictionary<string, string>
        {
            ["UserName"] = $"{user.FirstName} {user.LastName}",
            ["Email"] = user.Email,
            ["Password"] = newPassword,
            ["LoginUrl"] = $"{portalUrls.BaseUrl}{portalUrls.LoginUrl}?Email={user.Email}",
            ["SupportEmail"] = emailSettings.SupportEmail
        };

        var toEmails = new List<string> { user.Email };
        await _emailService.SendEmailAsync(
            EmailTemplate.UserCreation,
            placeholders,
            toEmails,
            "Login Details – Infinite Investments"
        );

        #region Activity
        var userName = $"{user.FirstName} {user.LastName}".Trim();
        await _activityService.AddActivityAsync(
            $"resent login email to {user.Role} \"{userName}\"",
            "User",
            userId);
        #endregion

        return Ok(new { Message = "Login email resent successfully." });
    }
}
