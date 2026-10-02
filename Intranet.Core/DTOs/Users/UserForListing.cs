using Intranet.Domain.Enums;
using System.Text.Json.Serialization;

namespace Intranet.Core.DTOs.Users;

public class UserForListing
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public int? AvatarAttachmentId { get; set; }
    public string? DisplayColor { get; set; }
    // Address
    public string? StreetAddress { get; set; }
    public string? Suburb { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CompanyName { get; set; }
}

