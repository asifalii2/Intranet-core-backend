using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Users;

public class UserStatusForUpdate
{
    [Required]
    public int Id { get; set; }

    [Required]
    public bool IsActive { get; set; }
}

