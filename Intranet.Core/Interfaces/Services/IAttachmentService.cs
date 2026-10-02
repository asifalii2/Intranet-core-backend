using Intranet.Core.DTOs.Attachments;
using Intranet.Core.DTOs.Paginations;

namespace Intranet.Core.Interfaces.Services;

public interface IAttachmentService
{
    Task<AttachmentDto> AddAttachmentAsync(AttachmentForCreation attachment);
    Task<PreSignedUrlDto> GetPreSignedUrlAsync(int attachmentId, int expirationMinutes = 5);
    Task<Domain.Entities.Attachment?> GetAttachmentByIdAsync(int id);
    Task<PagedList<Domain.Entities.Attachment>> GetAttachmentsAsync(AttachmentsRequestFilter filter);
    Task<bool> DeleteAttachmentAsync(int id);
    Task<bool> IsAttachmentExistAsync(int id);
    Task<bool> UpdateAttachmentAsync(AttachmentForUpdate attachmentForUpdate);
    Task<(Stream Stream, string ContentType, string FileName)> DownloadAttachmentAsync(int attachmentId);


}
