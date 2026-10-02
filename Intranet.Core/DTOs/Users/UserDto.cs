using Intranet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public int? AvatarAttachmentId { get; set; }
    public string? DisplayColor { get; set; }
    /// <summary>
    /// Role of the user (SuperAdmin, InternalUser, Consultant, Client, ProjectManager).
    /// </summary>
    public string Role { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailTwoFactorEnabled { get; set; }
    public bool IsPhoneTwoFactorEnabled { get; set; }
    // Address
    public string? StreetAddress { get; set; }
    public string? Suburb { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CompanyName { get; set; }
}

