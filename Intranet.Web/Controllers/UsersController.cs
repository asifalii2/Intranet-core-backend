using AutoMapper;
using Intranet.Core.Interfaces.Services;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Users;
using Intranet.Core.Enums;
using Intranet.Core.Helpers;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Intranet.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Intranet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;
        private readonly IAuthUser _authUser;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IActivityService _activityService;
        public UsersController(IUserService userService, IMapper mapper, ISettingsService settingsService, IAuthUser authUser, IEmailService emailService, IEmailTemplateService emailTemplateService, IActivityService activityService)
        {
            _userService = userService;
            _mapper = mapper;
            _settingsService = settingsService;
            _authUser = authUser;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _activityService = activityService;
        }
        [HttpGet]
        public async Task<ActionResult<List<UserForListing>>> GetUsersAsync([FromQuery] UsersRequestFilter? filter)
        {
            var users = await _userService.GetUsersAsync(filter);

            Response.AddPaginationHeader(users);

            var usersDto = _mapper.Map<List<UserForListing>>(users);

            return Ok(usersDto);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync([FromRoute] int userId)
        {
            if (userId == 1 && _authUser.UserId != 1)
                return NotFound(new { Message = "User Not Found" });

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            var usersDto = _mapper.Map<UserDto>(user);

            return Ok(usersDto);
        }

        [HttpGet("user-exits-by-email/{email}")]
        public async Task<ActionResult<bool>> GetUserByEmailAsync([FromRoute] string email)
        {
            var isExist = await _userService.IsUserEmailExistAsync(email);
            if (isExist)
                return BadRequest(new { Message = $"{email} already exists." });
            else
                return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> AddUserAsync(UserForCreation userForCreation)
        {
            if (userForCreation.Role == RoleConstants.SuperAdmin && _authUser.Role != RoleConstants.SuperAdmin)
                return BadRequest(new { Message = "Your are not allowed to set role as Admin." });

            var isExist = await _userService.IsUserEmailExistAsync(userForCreation.Email);
            if (isExist)
                return Conflict(new { Message = "This Email already exists." });

            var id = await _userService.AddUserAsync(userForCreation);

            #region Activity
            // Track activity with user name and role
            var userName = $"{userForCreation.FirstName} {userForCreation.LastName}".Trim();

            await _activityService.AddActivityAsync(
                $"created new {userForCreation.Role} \"{userName}\"",
                "User",
                id);
            #endregion

            return Ok(new
            {
                Message = id
            });
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync(UserForUpdate userForUpdation)
        {
            if (userForUpdation.Id == 1 && _authUser.UserId != 1)
                return BadRequest(new { Message = "Cannot update this user" });

            if (userForUpdation.Role == RoleConstants.SuperAdmin && _authUser.Role != RoleConstants.SuperAdmin)
                return BadRequest(new { Message = "Your are not allowed to update role as Admin." });

            var user = await _userService.GetUserByIdAsync(userForUpdation.Id);
            if (user is null)
                return NotFound(new { Message = "User not found." });

            await _userService.UpdateUserAsync(userForUpdation);

            #region Activity
            // Track activity with user name and role
            var userName = $"{userForUpdation.FirstName} {userForUpdation.LastName}".Trim();
            await _activityService.AddActivityAsync(
                $"updated {user.Role} \"{userName}\"",
                "User",
                userForUpdation.Id);
            #endregion

            return Ok(new { Message = "Updated Successfully." });
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUserAsync([FromRoute] int userId)
        {
            if (userId == 1)
                return NotFound(new { Message = "Cannot delete this user." });

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User Not Found" });

            if (user.Id == _authUser.UserId)
                return BadRequest(new { Message = "You Can Not Delete This user" });

            // Get user info before deletion for activity
            var userName = $"{user.FirstName} {user.LastName}".Trim();

            var isDeleted = await _userService.DeleteUserAsync(userId);
            if (!isDeleted)
                return BadRequest(new { Message = "Error in User Deletion." });

            #region Activity
            // Track activity with user name and role
            await _activityService.AddActivityAsync(
                $"deleted {user.Role} \"{userName}\"",
                "User",
                userId);
            #endregion

            return Ok(new { Message = "Deleted Successfully." });
        }

        [HttpDelete("soft-delete/{userId}")]
        public async Task<ActionResult> SoftDeleteUserAsync([FromRoute] int userId)
        {
            if (userId == 1)
                return NotFound(new { Message = "Cannot delete this user." });

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User Not Found" });

            if (user.Id == _authUser.UserId)
                return BadRequest(new { Message = "User Can Not Delete This user" });

            // Get user info before deletion for activity
            var userName = $"{user.FirstName} {user.LastName}".Trim();

            var userDeleted = await _userService.SoftDeleteUserAsync(userId);
            if (!userDeleted)
                return BadRequest(new { Message = "Error In User Deletion." });

            #region Activity
            // Track activity with user name and role
            await _activityService.AddActivityAsync(
                $"deleted {user.Role} \"{userName}\"",
                "User",
                userId);
            #endregion

            return Ok(new { Message = "Moved To Trash Successfully." });
        }

        [HttpDelete("bulk-soft-delete")]
        public async Task<ActionResult> SoftBulkUsersDeleteAsync(UsersForBulkDelete usersBulkForDelete)
        {
            if (usersBulkForDelete.Ids == null || !usersBulkForDelete.Ids.Any())
                return BadRequest(new { Message = "No user ids provided." });

            // Prevent deleting super admin
            if (usersBulkForDelete.Ids.Contains(1) && _authUser.UserId != 1)
                return BadRequest(new { Message = "Cannot delete super admin." });

            // Prevent deleting self
            if (usersBulkForDelete.Ids.Contains(_authUser.UserId))
                return BadRequest(new { Message = "User cannot delete themselves." });

            // Fetch users
            var users = await _userService.GetUsersByIdsAsync(usersBulkForDelete.Ids);

            if (!users.Any())
                return NotFound(new { Message = "Users Not Found." });

            // Perform bulk soft delete
            var deleted = await _userService.SoftDeleteBulkUsersAsync(usersBulkForDelete.Ids);
            if (!deleted)
                return BadRequest(new { Message = "Error in deleting users." });

            #region Activity
            var userNamesList = users.Select(x => $"{x.FirstName} {x.LastName}".Trim()).ToList();
            var userCount = userNamesList.Count;

            var userNamesForActivity = userCount switch
            {
                0 => "no users",
                1 => userNamesList[0],
                2 => $"{userNamesList[0]} and {userNamesList[1]}",
                _ => string.Join(", ", userNamesList.Take(userCount - 1)) + $", and {userNamesList.Last()}"
            };

            var userWord = userCount == 1 ? "user" : "users";

            await _activityService.AddActivityAsync(
                $"deleted {userWord} {userNamesForActivity}",
                "User",
                userNamesForActivity
            );
            #endregion

            return Ok(new { Message = "Users moved to trash successfully." });
        }

        [HttpPut("update-user-status")]
        public async Task<ActionResult> UpdateUserStatusAsync(
            UserStatusForUpdate userStatusForUpdate)
        {
            if (userStatusForUpdate.Id == 1 && _authUser.UserId != 1)
                return NotFound(new { Message = "Cannot update this user." });

            var isExist = await _userService.IsUserExistAsync(userStatusForUpdate.Id);

            if (!isExist)
                return NotFound(new { Message = "User Not Found." });

            // Get user info before update to capture previous status
            var user = await _userService.GetUserByIdAsync(userStatusForUpdate.Id);
            var previousStatus = user?.IsActive == true ? "active" : "inactive";
            var newStatus = userStatusForUpdate.IsActive ? "active" : "inactive";
            var userName = user != null
                ? $"{user.FirstName} {user.LastName}".Trim()
                : $"User #{userStatusForUpdate.Id}";

            var updated = await _userService.UpdateUserStatusAsync(userStatusForUpdate);
            if (!updated)
                return BadRequest(new { Message = "Error In User Status Update." });

            #region Activity
            // Track activity with user name, role, and status change
            var userRole = user?.Role ?? "user";
            await _activityService.AddActivityAsync(
                $"updated {userRole} \"{userName}\" status from {previousStatus} to {newStatus}",
                "User",
                userStatusForUpdate.Id);
            #endregion

            #region Email
            if (!user.HasLoginEmailSent && userStatusForUpdate.IsActive)
            {
                string password = "Intranet@" + UniqueNumberGenerator.Generate();
                user.PasswordHash = PasswordHasher.HashPassword(password);
                user.HasLoginEmailSent = true;
                await _userService.SaveChangesAsync();

                var portalUrls = _settingsService.GetFrontendPortalUrlSettings();
                var emailSettings = _settingsService.GetEmailSettings();
                var placeHoleders = new Dictionary<string, string>
                {
                    ["UserName"] = $"{user.FirstName} {user.LastName}",
                    ["Email"] = user.Email,
                    ["Password"] = password,
                    ["LoginUrl"] = $"{portalUrls.BaseUrl}{portalUrls.LoginUrl}?Email={user.Email}",
                    ["SupportEmail"] = emailSettings.SupportEmail
                };
                var toEmails = new List<string> { user.Email };
                var subject = "Welcome to Infinite Investments";

                await _emailService.SendEmailAsync(EmailTemplate.UserCreation, placeHoleders, toEmails, subject);
            }

            #endregion

            return Ok(new { Message = "Status Updated Successfully." });
        }

        [HttpPut("bulk-update-users-status")]
        public async Task<ActionResult> UpdateUsersStatusBulkAsync(UsersStatusForBulkUpdate bulkUserStatusForUpdate)
        {
            if (bulkUserStatusForUpdate.Ids == null || !bulkUserStatusForUpdate.Ids.Any())
                return BadRequest(new { Message = "No user ids provided." });

            // Prevent updating super admin
            if (bulkUserStatusForUpdate.Ids.Contains(1) && _authUser.UserId != 1)
                return NotFound(new { Message = "Cannot update this user." });

            var users = await _userService.GetUsersByIdsAsync(bulkUserStatusForUpdate.Ids);

            if (!users.Any())
                return NotFound(new { Message = "Users Not Found." });

            var usersToChange = users.Where(u => u.IsActive != bulkUserStatusForUpdate.IsActive);
            if (!usersToChange.Any())
                return Ok(new { Message = "Users Status Updated Successfully." });

            bulkUserStatusForUpdate.Ids = usersToChange.Select(x => x.Id).ToList();

            await _userService.UpdateUsersStatusBulkAsync(bulkUserStatusForUpdate);


            #region Activity
            var newStatus = bulkUserStatusForUpdate.IsActive ? "active" : "Inactive";
            var previousStatus = !bulkUserStatusForUpdate.IsActive ? "active" : "Inactive";

            var userNamesList = usersToChange.Select(x => $"{x.FirstName} {x.LastName}".Trim()).ToList();
            var userCount = userNamesList.Count;

            var userNamesForActivity = userCount switch
            {
                0 => "no users",
                1 => userNamesList[0],
                2 => $"{userNamesList[0]} and {userNamesList[1]}",
                _ => string.Join(", ", userNamesList.Take(userCount - 1)) + $", and {userNamesList.Last()}"
            };

            var userWord = userCount == 1 ? "user" : "users";

            await _activityService.AddActivityAsync(
                $"updated status of {userWord} {userNamesForActivity} from {previousStatus} to {newStatus}",
                "User",
                userNamesForActivity
            );
            #endregion

            #region Email
            if (bulkUserStatusForUpdate.IsActive)
            {
                var portalUrls = _settingsService.GetFrontendPortalUrlSettings();
                var emailSettings = _settingsService.GetEmailSettings();

                var usersToEmail = usersToChange.Where(u => !u.HasLoginEmailSent).ToList();

                foreach (var user in usersToEmail)
                {
                    string password = "Intranet@" + UniqueNumberGenerator.Generate();
                    user.PasswordHash = PasswordHasher.HashPassword(password);
                    user.HasLoginEmailSent = true;

                    var placeholders = new Dictionary<string, string>
                    {
                        ["UserName"] = $"{user.FirstName} {user.LastName}",
                        ["Email"] = user.Email,
                        ["Password"] = password,
                        ["LoginUrl"] = $"{portalUrls.BaseUrl}{portalUrls.LoginUrl}?Email={user.Email}",
                        ["SupportEmail"] = emailSettings.SupportEmail
                    };

                    var toEmails = new List<string> { user.Email };
                    var subject = "Welcome to Infinite Investments";

                    await _emailService.SendEmailAsync(EmailTemplate.UserCreation, placeholders, toEmails, subject);
                }

                if (usersToEmail.Any())
                    await _userService.SaveChangesAsync();
            }
            #endregion


            return Ok(new { Message = "Users Status Updated Successfully." });
        }


        [HttpPut("update-two-factor-status-for-phone")]
        public async Task<ActionResult> UpdateUserPhoneTwoFactorStatusAsync(
            UserPhoneTwoFactorEnabledStatusForUpdate userPhoneTwoFactorEnabledStatusForUpdate)
        {
            var updated = await _userService.UpdateUserPhoneTwoFactorEnabledStatusAsync(userPhoneTwoFactorEnabledStatusForUpdate);
            if (!updated)
                return BadRequest(new { Message = "Error In User Phone Status Update." });
            return Ok(new { Message = "Phone Status Updated Successfully." });
        }

        [HttpPut("update-two-factor-status-for-email")]
        public async Task<ActionResult> UpdateUserEmailStatusAsync(
            UserEmailTwoFactorEnabledStatusForUpdate userEmailTwoFactorEnabledStatusForUpdate)
        {
            var updated = await _userService.UpdateUserEmailTwoFactorEnabledStatusAsync(userEmailTwoFactorEnabledStatusForUpdate);
            if (!updated)
                return BadRequest(new { Message = "Error In User Email Status Update." });
            return Ok(new { Message = "Email Status Updated Successfully." });
        }
    }
}
