using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Users
{
    public class UsersStatusForBulkUpdate
    {
        [Required]
        public List<int> Ids { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
