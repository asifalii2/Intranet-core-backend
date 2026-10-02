using Intranet.Core.DTOs.Attachments;
using Intranet.Core.DTOs.Paginations;
using Intranet.Domain.Entities;

namespace Intranet.Core.Interfaces.Repositories;

public interface IAttachmentRepository
{
    Task<int> AddAttachmentAsync(Attachment attachment);
    Task<Attachment?> GetAttachmentByIdAsync(int id, string userRole);
    Task<bool> UpdateAttachmentAsync(Attachment attachment);
    Task<bool> DeleteAttachmentAsync(int id);
    Task<bool> IsAttachmentExistAsync(int id);
    Task<PagedList<Attachment>> GetAttachmentsAsync(AttachmentsRequestFilter filter, string userRole);
    Task<bool> UpdateAttachmentAsync(AttachmentForUpdate attachmentForUpdate);
}
