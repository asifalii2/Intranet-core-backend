using Intranet.Core.DTOs.Paginations;
using Intranet.Domain.Enums;

namespace Intranet.Core.DTOs.Users
{
    public class UsersRequestFilter : Pagination
    {
        public string? Search { get; set; }

        public string? Role { get; set; }   
        public bool? IsActive { get; set; }
    }
}



