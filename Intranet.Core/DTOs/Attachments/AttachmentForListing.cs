namespace Intranet.Core.DTOs.Attachments;

public class AttachmentForListing
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string FileExtension { get; set; }
    public long FileSize { get; set; }
    public string Visibility { get; set; }
}

