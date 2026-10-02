
using Intranet.Core.DTOs.Paginations;

namespace Intranet.Core.DTOs.Attachments;

public class AttachmentsRequestFilter : Pagination
{
    public string? Visibility { get; set; }
    public string? FileType { get; set; }
    public string? Search { get; set; }
}

