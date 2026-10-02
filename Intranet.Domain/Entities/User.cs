using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Intranet.Domain.Entities
{
    [Index(nameof(User.Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public int? AvatarAttachmentId { get; set; }
        public string? DisplayColor { get; set; }

        // Navigation property
        public Attachment? AvatarAttachment { get; set; }
        /// <summary>
        /// Role of the user (SuperAdmin, InternalUser, Consultant, Client, ProjectManager).
        /// </summary>
        public string Role { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public bool HasLoginEmailSent { get; set; }

        // Address
        public string? StreetAddress { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        // Fields for reset password
        public bool IsEmailTwoFactorEnabled { get; set; }
        public bool IsPhoneTwoFactorEnabled { get; set; }
        public int? EmailOTP { get; set; }
        public DateTime? OtpSendAt { get; set; }
        public int? PhoneOTP { get; set; }
        public int OtpFailedAttempts { get; set; }
        //public DateTime? PhoneOtpSendAt { get; set; }

        public string? CompanyName { get; set; }

        // Login attempt tracking
        public int LoginFailedAttempts { get; set; } = 0;
        public DateTime? LoginLockoutUntil { get; set; }

        // Base properties
        public DateTime CreatedAt { get; set; }
        public int? CreatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedById { get; set; }
    }
}
