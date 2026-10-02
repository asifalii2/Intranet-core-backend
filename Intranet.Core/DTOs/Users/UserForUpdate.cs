using Intranet.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Users
{
    public class UserForUpdate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        /// <summary>
        /// Role of the user (SuperAdmin).
        /// </summary>
        public string Role { get; set; }
        public int? AvatarAttachmentId { get; set; }
        // Address
        public string? StreetAddress { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? CompanyName { get; set; }
    }
}
