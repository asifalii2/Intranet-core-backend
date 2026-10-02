using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Users
{
    public class UsersForBulkDelete
    {
        [Required]
        public List<int> Ids { get; set; }
    }
}
