using System.ComponentModel.DataAnnotations;

namespace Intranet.Core.DTOs.Attachments;

public class AttachmentForUpdate
{
    [Required]
    public int Id { get; set; }
    public string? Visibility { get; set; }
}

