namespace Intranet.Core.DTOs.Attachments;

public class PreSignedUrlDto
{
    public string PreSignedUrl { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; }
}
