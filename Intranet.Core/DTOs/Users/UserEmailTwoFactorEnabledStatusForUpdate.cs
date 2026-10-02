using System.ComponentModel.DataAnnotations;
namespace Intranet.Core.DTOs.Users
{
    public class UserEmailTwoFactorEnabledStatusForUpdate
    {
        [Required]
        public bool IsEmailTwoFactorEnabled { get; set; }
    }
}
