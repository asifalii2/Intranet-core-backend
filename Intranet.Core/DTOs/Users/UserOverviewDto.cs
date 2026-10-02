
namespace Intranet.Core.DTOs.Users;

public class UserOverviewDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public int? AvatarAttachmentId { get; set; }
    public string? DisplayColor { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
}

