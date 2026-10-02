using AutoMapper;
using Intranet.Core.DTOs.Attachments;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;

namespace Intranet.Core.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IAuthUser _authUser;
    private readonly IMapper _mapper;

    public AttachmentService(
        IAttachmentRepository attachmentRepository,
        IAuthUser authUser,
        IMapper mapper)
    {
        _attachmentRepository = attachmentRepository;
        _authUser = authUser;
        _mapper = mapper;
    }

    public async Task<AttachmentDto> AddAttachmentAsync(AttachmentForCreation attachmentforCreation)
    {
        using var fileStream = attachmentforCreation.File.OpenReadStream();

        var attachment = _mapper.Map<Attachment>(attachmentforCreation);

        // Save to database
        var attachmentId = await _attachmentRepository.AddAttachmentAsync(attachment);

        return _mapper.Map<AttachmentDto>(attachment);
    }

    public async Task<PreSignedUrlDto> GetPreSignedUrlAsync(int attachmentId, int expirationMinutes = 5)
    {
        var attachment = await _attachmentRepository.GetAttachmentByIdAsync(attachmentId, _authUser.Role);

        if (attachment == null)
            throw new KeyNotFoundException($"Attachment with id {attachmentId} not found.");

        // Get pre-signed URL from S3
        //var preSignedUrl = await _fileStorageService.GetPreSignedUrlAsync(attachment.FileUrl, expirationMinutes);

        return new PreSignedUrlDto
        {
            PreSignedUrl = null, // TODO : Modify the code as your requrinment
            ExpiresInMinutes = expirationMinutes
        };
    }

    public async Task<Attachment?> GetAttachmentByIdAsync(int id)
    {
        return await _attachmentRepository.GetAttachmentByIdAsync(id, _authUser.Role);
    }

    public async Task<PagedList<Attachment>> GetAttachmentsAsync(AttachmentsRequestFilter filter)
    {
        return await _attachmentRepository.GetAttachmentsAsync(filter, _authUser.Role);
    }

    public async Task<bool> DeleteAttachmentAsync(int id)
    {
        // Get attachment with visibility rules applied
        var attachment = await _attachmentRepository.GetAttachmentByIdAsync(id, _authUser.Role);
        if (attachment == null)
            return false;

        // Delete attachment record from database
        return await _attachmentRepository.DeleteAttachmentAsync(id);
    }

    public async Task<bool> IsAttachmentExistAsync(int id)
    {
        return await _attachmentRepository.IsAttachmentExistAsync(id);
    }

    public async Task<bool> UpdateAttachmentAsync(AttachmentForUpdate attachmentForUpdate)
    {
        return await _attachmentRepository.UpdateAttachmentAsync(attachmentForUpdate);
    }

    public async Task<(Stream Stream, string ContentType, string FileName)> DownloadAttachmentAsync(int attachmentId)
    {
        var attachment = await _attachmentRepository.GetAttachmentByIdAsync(attachmentId, _authUser.Role);

        if (attachment == null)
            throw new KeyNotFoundException($"Attachment with id {attachmentId} not found.");

        return (Stream: null, ContentType: attachment.FileType, FileName: attachment.FileName); // TODO : Modify the code as your requrinment

    }
}
