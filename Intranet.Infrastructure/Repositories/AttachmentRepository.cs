using AutoMapper;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Attachments;
using Intranet.Core.DTOs.Paginations;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Domain.Entities;
using Intranet.Infrastructure.Contexts;
using Intranet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Intranet.Infrastructure.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly IntranetDBContext _context;
    private readonly IMapper _mapper;
    private readonly IAuthUser _authUser;

    public AttachmentRepository(IntranetDBContext context, IMapper mapper, IAuthUser authUser)
    {
        _context = context;
        _mapper = mapper;
        _authUser = authUser;   
    }

    public async Task<int> AddAttachmentAsync(Attachment attachment)
    {
        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment.Id;
    }

    public async Task<Attachment?> GetAttachmentByIdAsync(int id, string userRole)
    {
        var query = _context.Attachments.Where(x => x.Id == id && !x.IsDeleted);

        // Filter by visibility: SuperAdmin can see all, others see only if their role is in Visibility or Public is in Visibility
        if (userRole != RoleConstants.SuperAdmin)
        {
            query = query.Where(x => x.Visibility.Contains("Public") || x.Visibility.Contains(userRole));
        }
        
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAttachmentAsync(Attachment attachment)
    {
        var existingAttachment = await _context.Attachments
            .FirstOrDefaultAsync(x => x.Id == attachment.Id);
        
        if (existingAttachment == null)
            return false;

        _mapper.Map(attachment, existingAttachment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAttachmentAsync(int id)
    {
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        
        if (attachment == null)
            return false;

        _context.Attachments.Remove(attachment);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<PagedList<Attachment>> GetAttachmentsAsync(AttachmentsRequestFilter filter, string userRole)
    {
        var query = _context.Attachments
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        // Filter by visibility: SuperAdmin can see all, others see only if their role is in Visibility or Public is in Visibility
        if (userRole != RoleConstants.SuperAdmin)
        {
            query = query.Where(x => x.Visibility.Contains("Public") || x.Visibility.Contains(userRole));
        }

        if (!string.IsNullOrWhiteSpace(filter.Visibility))
            query = query.Where(x => x.Visibility.Contains(filter.Visibility));

        if (!string.IsNullOrWhiteSpace(filter.FileType))
            query = query.Where(x => x.FileType == filter.FileType);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(x => 
                x.FileName.Contains(filter.Search) ||
                (x.FileType != null && x.FileType.Contains(filter.Search)) ||
                (x.FileExtension != null && x.FileExtension.Contains(filter.Search))
            );

        var attachments = await query.ToPagedListAsync(filter);

        return attachments;
    }

    public async Task<bool> IsAttachmentExistAsync(int id)
    {
        return await _context.Attachments
            .AnyAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> UpdateAttachmentAsync(AttachmentForUpdate attachmentForUpdate)
    {
        var existingAttachment = await _context.Attachments
            .FirstOrDefaultAsync(x => x.Id == attachmentForUpdate.Id && !x.IsDeleted);
        
        if (existingAttachment == null)
            return false;

        _mapper.Map(attachmentForUpdate, existingAttachment);
        await _context.SaveChangesAsync();
        return true;
    }
}