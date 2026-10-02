using Intranet.Core.Constants;
using Intranet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Intranet.Core.DTOs.Users
{
    public class UserForCreation
    {
        [Required]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public int? AvatarAttachmentId { get; set; }
        /// <summary>
        /// Role of the user (SuperAdmin, InternalUser, Consultant, Client, ProjectManager).
        /// </summary>
        public string Role { get; set; }
        public string? Password { get; set; }
        public string? CompanyName { get; set; }
        // Address
        public string? StreetAddress { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}
