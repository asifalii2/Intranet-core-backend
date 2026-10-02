using Intranet.Core.DTOs.Base;

namespace Intranet.Core.DTOs.Attachments;

public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string FileExtension { get; set; }
    public long FileSize { get; set; }
    public string Visibility { get; set; }
    public int? JobId { get; set; }
    public int? JobTaskId { get; set; }
}

