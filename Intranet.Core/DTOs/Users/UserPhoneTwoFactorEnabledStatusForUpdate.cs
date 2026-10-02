
using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Users
{
    public class UserPhoneTwoFactorEnabledStatusForUpdate
    {
        [Required]
        public bool IsPhoneTwoFactorEnabled { get; set; }
    }
}
